using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct AimedTargetAspectRO : IAspect {
    private readonly RefRO<LocalTransform>  _LocTrans;
    private readonly RefRO<AimedTargetData> _AimedTargetData;

    [ReadOnly] private readonly DynamicBuffer<StatsBuffer> _Stats;

    public ref readonly Entity Target => ref _AimedTargetData.ValueRO.target;

    public bool IsTargetExists(
        in EntityStorageInfoLookup     entityLookup
      , in ComponentLookup<Selectable> selectLookup) =>
        GameHelpers.IsTargetExists(
            _AimedTargetData.ValueRO.target
          , entityLookup
          , selectLookup);

    public bool IsTargetOutOfRange(
        int                                attackRangeId
      , int                                unitRadiusId
      , in ComponentLookup<LocalTransform> locTransLookup
      , in BufferLookup<StatsBuffer>       statsLookup) =>
        GameHelpers.IsTargetOutOfRange(
            _LocTrans.ValueRO.Position
          , locTransLookup[_AimedTargetData.ValueRO.target].Position
          , _Stats[attackRangeId].value
          , statsLookup[_AimedTargetData.ValueRO.target][unitRadiusId].value);

    public bool NeedMoveToTarget(
        in EntityStorageInfoLookup         entityLookup
      , in ComponentLookup<Selectable>     selectLookup
      , int                                attackRangeId
      , int                                unitRadiusId
      , in ComponentLookup<LocalTransform> locTransLookup
      , in BufferLookup<StatsBuffer>       statsLookup) =>
        IsTargetExists(entityLookup, selectLookup)
     && IsTargetOutOfRange(attackRangeId, unitRadiusId, locTransLookup, statsLookup); // Out range

    public bool HaveTargetInRange(
        in EntityStorageInfoLookup         entityLookup
      , in ComponentLookup<Selectable>     selectLookup
      , int                                attackRangeId
      , int                                unitRadiusId
      , in ComponentLookup<LocalTransform> locTransLookup
      , in BufferLookup<StatsBuffer>       statsLookup) =>
        IsTargetExists(entityLookup, selectLookup)
     && !IsTargetOutOfRange(attackRangeId, unitRadiusId, locTransLookup, statsLookup); // In range
}