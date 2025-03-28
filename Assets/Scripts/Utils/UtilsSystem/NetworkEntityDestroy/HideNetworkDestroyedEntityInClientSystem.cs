using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
[UpdateAfter(typeof(PredictedSimulationSystemGroup))]
[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct HideNetworkDestroyedEntityInClientSystem : ISystem {
    private const float  BLACK_HOLE_DEEP = -1e9f;
    private       float3 _BlackHole;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        _BlackHole = new float3(0, BLACK_HOLE_DEEP, 0);
        
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAllRW<LocalTransform>()
            .WithAll<NetworkDestroyedTag>()
            .Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (var trans in SystemAPI
            .Query<RefRW<LocalTransform>>()
            .WithAll<NetworkDestroyedTag>())
            trans.ValueRW.Position = _BlackHole;
    }
}