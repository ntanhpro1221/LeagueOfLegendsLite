using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(HandleInOut_Damage_Exp_Gold_SystemGroup))]
public partial struct ApplyIncomingGoldSystem : ISystem {
    private ComponentLookup<GoldData> goldLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        goldLookup = SystemAPI.GetComponentLookup<GoldData>(
            isReadOnly: false);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        goldLookup.Update(ref state);

        state.Dependency = new Job {
            goldLookup = goldLookup
        }.Schedule(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public ComponentLookup<GoldData> goldLookup;

        [BurstCompile]
        private void Execute(in DynamicBuffer<OutgoingGoldBuffer> goldBuffer) {
            foreach (var gold in goldBuffer) goldLookup.GetRefRW(gold.target).ValueRW.gold += gold.gold;
        }
    }
}