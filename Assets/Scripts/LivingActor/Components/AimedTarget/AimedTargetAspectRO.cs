using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct AimedTargetAspectRO : IAspect {
    private readonly RefRO<LocalTransform>  _LocTrans;
    private readonly RefRO<AimedTargetData> _AimedTargetData;

    [ReadOnly] private readonly DynamicBuffer<StatsBuffer> _Stats;

    public ref readonly Entity Target => ref _AimedTargetData.ValueRO.target;

    // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
    public bool IsTargetExists(in EntityStorageInfoLookup lookup) =>
        lookup.Exists(_AimedTargetData.ValueRO.target);

    public bool IsTargetOutOfRange(
        int                                attackRangeId
      , int                                unitRadiusId
      , in ComponentLookup<LocalTransform> locTransLookup
      , in BufferLookup<StatsBuffer>       statsLookup) =>
        _Stats[attackRangeId].value + statsLookup[_AimedTargetData.ValueRO.target][unitRadiusId].value
      < math.length((
                _LocTrans.ValueRO.Position
              - locTransLookup[_AimedTargetData.ValueRO.target].Position)
            .WithoutY());

    public bool NeedMoveToTarget(
        in EntityStorageInfoLookup         entityLookup
      , int                                attackRangeId
      , int                                unitRadiusId
      , in ComponentLookup<LocalTransform> locTransLookup
      , in BufferLookup<StatsBuffer>       statsLookup) =>
        IsTargetExists(entityLookup)
     && IsTargetOutOfRange(attackRangeId, unitRadiusId, locTransLookup, statsLookup); // Out range

    public bool HaveTargetInRange(
        in EntityStorageInfoLookup         entityLookup
      , int                                attackRangeId
      , int                                unitRadiusId
      , in ComponentLookup<LocalTransform> locTransLookup
      , in BufferLookup<StatsBuffer>       statsLookup) =>
        IsTargetExists(entityLookup)
     && !IsTargetOutOfRange(attackRangeId, unitRadiusId, locTransLookup, statsLookup); // In range
}