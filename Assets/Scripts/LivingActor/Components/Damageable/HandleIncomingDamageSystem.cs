using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
public partial struct HandleIncomingDamageSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<
            IncomingDamageBuffer
          , HealthData
          , Simulate>().Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick) return;
        
        foreach (var (
                incomingDamage
              , healthData)
            in SystemAPI
                .Query<
                    DynamicBuffer<IncomingDamageBuffer>
                  , RefRW<HealthData>>()
                .WithAll<Simulate>()) {
            float totalDamage = 0f;
            foreach (var damage in incomingDamage)
                totalDamage += damage.damage;
            incomingDamage.Clear();

            healthData.ValueRW.value -= totalDamage;
        }
    }
}