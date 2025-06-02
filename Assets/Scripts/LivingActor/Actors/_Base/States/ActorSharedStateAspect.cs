using Unity.Entities;
using Unity.Transforms;

public readonly partial struct ActorSharedStateAspect : IAspect {
    [Optional] private readonly EnabledRefRW<IdleState>   _IdleState;
    [Optional] private readonly EnabledRefRW<AttackState> _AttackState;
    [Optional] private readonly EnabledRefRW<DeadState>   _DeadState;
    [Optional] private readonly EnabledRefRW<FreezeState> _FreezeState;
    [Optional] private readonly EnabledRefRW<MoveState>   _MoveState;

    [Optional] private readonly EnabledRefRW<Dead2IdleState> _Dead2IdleState;
    [Optional] private readonly EnabledRefRW<Idle2DeadState> _Idle2DeadState;

    [Optional] private readonly EnabledRefRW<ItemActiveAnalyzingState> _ItemAnalyzingState;
    [Optional] private readonly EnabledRefRW<Skill_Q_State>            _Skill_Q_State;
    [Optional] private readonly EnabledRefRW<Skill_W_State>            _Skill_W_State;
    [Optional] private readonly EnabledRefRW<Skill_E_State>            _Skill_E_State;
    [Optional] private readonly EnabledRefRW<Skill_R_State>            _Skill_R_State;

    public void SetIdle()   => _IdleState.ValueRW = true;
    public void SetAttack() => _AttackState.ValueRW = true;
    public void SetDead()   => _DeadState.ValueRW = true;
    public void SetFreeze() => _FreezeState.ValueRW = true;
    public void SetMove()   => _MoveState.ValueRW = true;

    public void SetDead2Idle() => _Dead2IdleState.ValueRW = true;
    public void SetIdle2Dead() => _Idle2DeadState.ValueRW = true;

    public void SetItemActiveAnalyzing()   => _ItemAnalyzingState.ValueRW = true;
    public void UnsetItemActiveAnalyzing() => _ItemAnalyzingState.ValueRW = false;
    public void SetSkill_Q()               => _Skill_Q_State.ValueRW = true;
    public void SetSkill_W()               => _Skill_W_State.ValueRW = true;
    public void SetSkill_E()               => _Skill_E_State.ValueRW = true;
    public void SetSkill_R()               => _Skill_R_State.ValueRW = true;

    private readonly RefRO<LocalTransform> _JustBecauseOfSyntax;
}