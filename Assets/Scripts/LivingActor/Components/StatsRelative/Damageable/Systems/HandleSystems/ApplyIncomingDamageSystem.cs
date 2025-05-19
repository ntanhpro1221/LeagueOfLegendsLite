using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(HandleIncomingDamageSystemGroup))]
public partial struct ApplyIncomingDamageSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<
            IncomingDamageBuffer
          , HealthData
          , Simulate>().Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job()
            .ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(HealthData))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(
            ref DynamicBuffer<IncomingDamageBuffer> incomingDamage
          , ref HealthData                          healthData) {
            float_Q3 totalDamage = 0;
            foreach (var damage in incomingDamage)
                totalDamage += damage.damage;
            healthData.value -= totalDamage;
        }
    }
}