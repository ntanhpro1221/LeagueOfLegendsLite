using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(NormalAttackSystemGroup))]
public partial struct CommonMeleeAttackSystem : ISystem {
    private BufferLookup<IncomingDamageBuffer> incomingDmgLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        incomingDmgLookup = SystemAPI.GetBufferLookup<IncomingDamageBuffer>(
            isReadOnly: false);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        incomingDmgLookup.Update(ref state);

        state.Dependency = new Job {
            incomingDmgLookup = incomingDmgLookup
        }.Schedule(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    public partial struct Job : IJobEntity {
        public BufferLookup<IncomingDamageBuffer> incomingDmgLookup;

        [BurstCompile]
        public void Execute(
            in AimedTargetData               target
          , in DynamicBuffer<StatsBuffer>    stats
          , EnabledRefRW<MeleeAttackTrigger> attackTrigger
          , in Entity                        entity) {

            incomingDmgLookup[target.target].Add(new IncomingDamageBuffer(stats[StatsId.PhysicDamage].value, entity));

            attackTrigger.ValueRW = false;
        }
    }
}