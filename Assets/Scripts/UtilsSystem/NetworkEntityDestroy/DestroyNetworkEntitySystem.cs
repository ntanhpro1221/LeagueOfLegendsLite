using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
public partial struct DestroyNetworkEntitySystem : ISystem {
    // to virtual destroy on client-side (hide from user's sight)
    // you cannot directly destroy entity from client side
    public const float BLACK_HOLE_Y = -1000;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<AutoDestroyNetworkEntityTag>()
            .WithNone<DestroyedInClientTag>()
            .Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick) return;

        using var ecb      = new EntityCommandBuffer(Allocator.Temp);
        bool      isClient = state.WorldUnmanaged.IsClient();

        foreach (var (
                localTrans
              , entity)
            in SystemAPI.Query<
                    RefRW<LocalTransform>>()
                .WithAll<
                    AutoDestroyNetworkEntityTag
                  , Simulate>()
                .WithNone<
                    DestroyedInClientTag>()
                .WithEntityAccess()) {
            if (isClient) {
                localTrans.ValueRW.Position = new float3(0, BLACK_HOLE_Y, 0);

                ecb.AddComponent<DestroyedInClientTag>(entity);
            }
            else ecb.DestroyEntity(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}