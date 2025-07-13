using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(HandleInOut_Damage_Exp_Gold_SystemGroup))]
public partial struct ApplyIncomingDamageSystem : ISystem {
    [ReadOnly] private ComponentLookup<ChampionTag> champLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        champLookup = SystemAPI.GetComponentLookup<ChampionTag>(
            isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        champLookup.Update(ref state);

        state.Dependency = new Job {
            champLookup = champLookup
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        [ReadOnly] public ComponentLookup<ChampionTag> champLookup;

        [BurstCompile]
        private void Execute(
            in  DynamicBuffer<IncomingDamageBuffer> incomingDamage
          , ref HealthData                          health
          , BountyAspectRW                          bounty) {

            float_Q3 totalDamage = 0;

            foreach (var damage in incomingDamage) totalDamage += damage.damage;

            var newHealth = health.value - totalDamage;

            if (health.value > 0 && newHealth <= 0)
                foreach (var damage in incomingDamage)
                    if (champLookup.HasComponent(damage.source)) {
                        bounty.TurnOn(damage.source);
                        break;
                    }

            health.value = newHealth;
        }
    }
}