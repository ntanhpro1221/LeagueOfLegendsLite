using Unity.Entities;
using Unity.Transforms;

public readonly partial struct ActorSharedStateAspect : IAspect {
    [Optional] private readonly EnabledRefRW<IdleState>   _IdleState;
    [Optional] private readonly EnabledRefRW<AttackState> _AttackState;
    [Optional] private readonly EnabledRefRW<DeadState>   _DeadState;
    [Optional] private readonly EnabledRefRW<FreezeState> _FreezeState;
    [Optional] private readonly EnabledRefRW<MoveState>   _MoveState;

    public void SetIdle()   => _IdleState.ValueRW = true;
    public void SetAttack() => _AttackState.ValueRW = true;
    public void SetDead()   => _DeadState.ValueRW = true;
    public void SetFreeze() => _FreezeState.ValueRW = true;
    public void SetMove()   => _MoveState.ValueRW = true;

    private readonly RefRO<LocalTransform> _JustBecauseOfSyntax;
}