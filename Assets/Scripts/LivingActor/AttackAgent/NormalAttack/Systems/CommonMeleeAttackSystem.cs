using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(NormalAttackSystemGroup))]
public partial struct CommonMeleeAttackSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EnumIndexData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var damageId = SystemAPI.GetSingleton<EnumIndexData>().StatsType[StatsType.PhysicDamage];

        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var data in SystemAPI.Query<UpdateAspect>()) {
            data.MarkAttackWasPerformed();
            
            ecb.AppendToBuffer(data.Target, new IncomingDamageBuffer(data.Damage(damageId)));
        }

        ecb.Playback(state.EntityManager);
    }

    private readonly partial struct UpdateAspect : IAspect {
        #pragma warning disable CS0414 // Field is assigned but its value is never used
        private readonly RefRO<Simulate> _Simulate;
        #pragma warning restore CS0414 // Field is assigned but its value is never used

        private readonly RefRO<AimedTargetData>           _AimedTarget;
        private readonly EnabledRefRW<MeleeAttackTrigger> _AttackTrigger;

        [ReadOnly] private readonly DynamicBuffer<StatsBuffer> _Stats;

        public Entity   Target                   => _AimedTarget.ValueRO.target;
        public float_Q3 Damage(int damageId)     => _Stats[damageId].value;
        public void     MarkAttackWasPerformed() => _AttackTrigger.ValueRW = false;
    }
}