using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
[UpdateBefore(typeof(DestroyNetworkEntityServerSystem))]
[UpdateBefore(typeof(HideNetworkDestroyedEntityInClientSystem))]
public partial struct DestroyAtDestinationSystem : ISystem {
    private const float DESTINATION_TOLERANCE = 0.001f;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();

        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<
                DestroyAtDestination
              , LocalToWorld
              , Simulate>()
            .Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick) return;

        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (
                destroyDes
              , localToWorld
              , entity)
            in SystemAPI.Query<
                    RefRO<DestroyAtDestination>
                  , RefRO<LocalToWorld>>()
                .WithAll<Simulate>()
                .WithEntityAccess()) {
            if (DESTINATION_TOLERANCE < math.distance(localToWorld.ValueRO.Position, destroyDes.ValueRO.destination)) continue;
            ecb.RemoveComponent<DestroyAtDestination>(entity);
            ecb.SetComponentEnabled<NetworkDestroyedTag>(entity, true);
        }

        ecb.Playback(state.EntityManager);
    }
}