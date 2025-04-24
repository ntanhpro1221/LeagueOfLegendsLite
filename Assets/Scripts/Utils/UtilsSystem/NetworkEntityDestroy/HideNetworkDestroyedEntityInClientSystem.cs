using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
[UpdateInGroup(typeof(DestroyNetworkEntitySystemGroup))]
public partial struct HideNetworkDestroyedEntityInClientSystem : ISystem {
    private static readonly float3 _BlackHole = -1e9f;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAllRW<LocalTransform>()
            .WithAll<
                Simulate
              , NetworkDestroyedTag>()
            .Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job()
            .ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(NetworkDestroyedTag))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(ref LocalTransform locTrans) {
            locTrans.Position = _BlackHole;
        }
    }
}