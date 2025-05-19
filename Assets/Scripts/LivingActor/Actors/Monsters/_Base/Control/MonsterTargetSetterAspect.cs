using Unity.Entities;

public readonly partial struct MonsterTargetSetterAspect : IAspect {
    private readonly RefRW<AimedTargetData> _Target;

    [Optional] private readonly EnabledRefRW<MonsterLeashAnchor> _AnchorTrigger;

    public bool IsTargetExists(in ComponentLookup<Selectable> selectLookup) =>
        GameHelpers.IsTargetExists(_Target.ValueRO.target, selectLookup);
    
    public void SetTargetUnsafe(in Entity target) {
        _Target.ValueRW.target = target;
        _AnchorTrigger.ValueRW = true;
    }

    public bool TrySetTarget(in Entity target, in ComponentLookup<Selectable> selectLookup) {
        if (GameHelpers.IsTargetExists(target, selectLookup)) {
            SetTargetUnsafe(target);
            return true;
        } else return false;
    }
}