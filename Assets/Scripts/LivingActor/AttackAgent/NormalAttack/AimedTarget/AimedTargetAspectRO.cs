using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct AimedTargetAspectRO : IAspect {
    private readonly RefRO<LocalToWorld>    _LocalToWorld;
    private readonly RefRO<AimedTargetData> _AimedTargetData;

    [ReadOnly] private readonly DynamicBuffer<StatsBuffer> _Stats;

    public ref readonly Entity Target => ref _AimedTargetData.ValueRO.target;

    // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
    public bool IsTargetExists(in EntityStorageInfoLookup lookup) =>
        lookup.Exists(_AimedTargetData.ValueRO.target);

    public bool IsTargetOutOfRange(int attackRangeId, in ComponentLookup<LocalToWorld> l2wLookup) =>
        _Stats[attackRangeId].value
      < math.length((
                _LocalToWorld.ValueRO.Position
              - l2wLookup[_AimedTargetData.ValueRO.target].Position)
            .WithoutY());

    public bool NeedMoveToTarget(
        in EntityStorageInfoLookup       lookup
      , int                              attackRangeId
      , in ComponentLookup<LocalToWorld> l2wLookup) =>
        IsTargetExists(lookup)
     && IsTargetOutOfRange(attackRangeId, l2wLookup); // Out range
    
    public bool HaveTargetInRange(
        in EntityStorageInfoLookup       lookup
      , int                              attackRangeId
      , in ComponentLookup<LocalToWorld> l2wLookup) =>
        IsTargetExists(lookup)
     && !IsTargetOutOfRange(attackRangeId, l2wLookup); // In range
}