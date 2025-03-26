using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct HandleDestroyAtDestinationSystem : ISystem {
    public const float DESTINATION_TOLERANCE = 0.0001f;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();

        state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<
            DestroyAtDestination
          , LocalToWorld
          , Simulate>().Build());
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
                .WithNone<AutoDestroyNetworkEntityTag>()
                .WithEntityAccess()) {
            if (destroyDes.ValueRO.destination.Equals(float3.zero)) {
                if (state.WorldUnmanaged.IsClient())
                    Debug.LogWarning($"NGDtuanh: Destination to destroy is default value, it's not valid | {entity} | Client");
                else 
                    Debug.LogWarning($"NGDtuanh: Destination to destroy is default value, it's not valid | {entity} | Server");
                continue;
            }

            if (DESTINATION_TOLERANCE > math.distance(localToWorld.ValueRO.Position, destroyDes.ValueRO.destination)) {
                ecb.RemoveComponent<DestroyAtDestination>(entity);
                ecb.AddComponent<AutoDestroyNetworkEntityTag>(entity);
            }
        }

        ecb.Playback(state.EntityManager);
    }
}