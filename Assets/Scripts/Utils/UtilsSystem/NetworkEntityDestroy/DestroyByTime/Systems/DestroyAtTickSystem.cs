using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(BeforeDestroyNetworkEntitySystemGroup))]
public partial struct DestroyAtTickSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();

        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<
                DestroyAtTick
              , Simulate>()
            .Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var netTime = SystemAPI.GetSingleton<NetworkTime>();
        if (!netTime.IsFirstTimeFullyPredictingTick) return;

        var curTick = netTime.ServerTick;

        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (
                destroyTick
              , entity)
            in SystemAPI
                .Query<RefRO<DestroyAtTick>>()
                .WithAll<Simulate>()
                .WithEntityAccess()) {
            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            if (destroyTick.ValueRO.tick.IsNewerThan(curTick)) continue;
            ecb.RemoveComponent<DestroyAtTick>(entity);
            ecb.SetComponentEnabled<NetworkDestroyedTag>(entity, true);
        }

        ecb.Playback(state.EntityManager);
    }
}