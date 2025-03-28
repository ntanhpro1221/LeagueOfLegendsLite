using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
[UpdateBefore(typeof(DestroyAtTickSystem))]
public partial struct InitDestroyAtTickSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<ClientServerTickRate>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<
                DestroyAtTickInitData
              , Simulate>()
            .Build());

        state.RequireForUpdate<NetworkTime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var netTime = SystemAPI.GetSingleton<NetworkTime>();
        if (!netTime.IsFirstTimeFullyPredictingTick) return;
        
        var curTick = netTime.ServerTick;
        var tickInterval = 1f / SystemAPI.GetSingleton<ClientServerTickRate>().SimulationTickRate;

        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (
                initData
              , entity)
            in SystemAPI
                .Query<
                    RefRO<DestroyAtTickInitData>>()
                .WithAll<Simulate>()
                .WithEntityAccess()) {
            var destroyTick = curTick;
            destroyTick.Add((uint)(initData.ValueRO.lifeTime / tickInterval));

            ecb.RemoveComponent<DestroyAtTickInitData>(entity);
            ecb.AddComponent(entity, new DestroyAtTick {
                tick = destroyTick
            });
        }

        ecb.Playback(state.EntityManager);
    }
}