using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(HandleInOut_Damage_Exp_Gold_SystemGroup))]
public partial struct NotifyWhenIWasBeatenSystem : ISystem {
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
        private void Execute(
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