using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct HandleDestroyAtTickSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<
            DestroyAtTick
          , Simulate>().Build());
        state.RequireForUpdate<NetworkTime>();
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
                .WithNone<AutoDestroyNetworkEntityTag>()
                .WithEntityAccess()) {
            if (!destroyTick.ValueRO.tick.IsValid) {
                state.EntityManager.GetName(entity, out var entityName);
                Debug.LogError($"Destroy tick is not valid at: {entityName} | {entity}");
                continue;
            }

            if (curTick.IsNewerThan(destroyTick.ValueRO.tick)) {
                ecb.RemoveComponent<DestroyAtTick>(entity);
                ecb.AddComponent<AutoDestroyNetworkEntityTag>(entity);
            }
        }
        
        ecb.Playback(state.EntityManager);
    }
}