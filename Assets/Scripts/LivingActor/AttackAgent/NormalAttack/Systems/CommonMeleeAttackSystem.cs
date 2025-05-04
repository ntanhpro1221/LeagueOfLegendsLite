using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(NormalAttackSystemGroup))]
public partial struct CommonMeleeAttackSystem : ISystem {
    private BufferLookup<IncomingDamageBuffer> incomingDmgLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EnumIndexData>();

        incomingDmgLookup = SystemAPI.GetBufferLookup<IncomingDamageBuffer>(
            isReadOnly: false);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        incomingDmgLookup.Update(ref state);

        state.Dependency = new Job {
            incomingDmgLookup = incomingDmgLookup
          , damageId          = SystemAPI.GetSingleton<EnumIndexData>().StatsType[StatsType.PhysicDamage]
        }.Schedule(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    public partial struct Job : IJobEntity {
        public BufferLookup<IncomingDamageBuffer> incomingDmgLookup;
        public int                                damageId;

        [BurstCompile]
        public void Execute(
            in AimedTargetData               target
          , in DynamicBuffer<StatsBuffer>    stats
          , EnabledRefRW<MeleeAttackTrigger> attackTrigger
          , in Entity entity) {

            incomingDmgLookup[target.target].Add(new IncomingDamageBuffer(stats[damageId].value, entity));

            attackTrigger.ValueRW = false;
        }
    }
}