using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(HandleIncomingDamageSystemGroup))]
public partial struct NotifyWhenIWasBeatenSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<
                IncomingDamageBuffer
              , HealthData
              , Simulate>()
            .WithPresent<BeBeaten>()
            .Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job()
            .ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(HealthData))]
    [WithPresent(typeof(BeBeaten))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(
            in  DynamicBuffer<IncomingDamageBuffer> incomingDamage
          , ref BeBeaten                            beBeatenData
          , EnabledRefRW<BeBeaten>                  beBeatenTrigger) {
            beBeatenTrigger.ValueRW = false;

            if (incomingDamage.IsEmpty) return;

            beBeatenTrigger.ValueRW = true;
            beBeatenData.source     = incomingDamage[0].source;
        }
    }
}