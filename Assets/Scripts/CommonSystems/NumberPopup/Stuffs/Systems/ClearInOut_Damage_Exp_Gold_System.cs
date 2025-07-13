using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(HandleInOut_Damage_Exp_Gold_SystemGroup), OrderLast = true)]
public partial struct ClearInOut_Damage_Exp_Gold_System : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new DamageJob().ScheduleParallel(state.Dependency);
        state.Dependency = new ExpJob().ScheduleParallel(state.Dependency);
        state.Dependency = new GoldJob().ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    private partial struct DamageJob : IJobEntity {
        [BurstCompile]
        private void Execute(ref DynamicBuffer<IncomingDamageBuffer> incomingDamage) {
            incomingDamage.Clear();
        }
    }

    [BurstCompile]
    private partial struct ExpJob : IJobEntity {
        [BurstCompile]
        private void Execute(ref DynamicBuffer<IncomingExpBuffer> incomingExp) {
            incomingExp.Clear();
        }
    }

    [BurstCompile]
    private partial struct GoldJob : IJobEntity {
        [BurstCompile]
        private void Execute(ref DynamicBuffer<OutgoingGoldBuffer> incomingGold) {
            incomingGold.Clear();
        }
    }
}