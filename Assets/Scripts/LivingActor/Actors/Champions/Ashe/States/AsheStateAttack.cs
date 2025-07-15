using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(StateUpdateSystemGroup))]
[UpdateAfter(typeof(ChampionStateAttack.Update))]
public partial struct UpdateAsheAttackAnimSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AsheTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (var (
            anim
          , effectMap
            ) in SystemAPI
            .Query<
                RefRW<SharedAnimData>
              , RefRO<EffectMap>
            >().WithAll<
                AsheTag
              , Simulate
              , AttackState
            >().WithAll<
                RangedAttackTrigger
            >())
            anim.ValueRW.curAnim = effectMap.ValueRO.ContainsKey(EffectId.AsheSkill_Q_Active)
                ? SharedAnimKey.Skill_Q
                : SharedAnimKey.Attack;
    }
}