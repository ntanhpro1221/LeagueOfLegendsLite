using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct AimedTargetAspectRO : IAspect {
  public const int CLOSE_TO_TARGET_TOLERANCE = 5;

  private readonly RefRO<LocalTransform>  _LocTrans;
  private readonly RefRO<AimedTargetData> _AimedTargetData;

  [ReadOnly] private readonly DynamicBuffer<StatsBuffer> _Stats;

  public ref readonly Entity Target => ref _AimedTargetData.ValueRO.target;

  public bool IsTargetExists(in ComponentLookup<Selectable> selectLookup) =>
    GameHelpers.IsTargetExists(
      _AimedTargetData.ValueRO.target
    , selectLookup);

  public bool IsTargetOutOfRange(
    in ComponentLookup<LocalTransform> locTransLookup
  , in BufferLookup<StatsBuffer>       statsLookup) =>
    GameHelpers.IsTargetOutOfRange(
      _LocTrans.ValueRO.Position
    , locTransLookup[_AimedTargetData.ValueRO.target].Position
    , _Stats[StatsId.AttackRange].value
    , statsLookup[_AimedTargetData.ValueRO.target][StatsId.UnitRadius].value);

  public bool NeedMoveToTarget(
    in ComponentLookup<Selectable>     selectLookup
  , in ComponentLookup<LocalTransform> locTransLookup
  , in BufferLookup<StatsBuffer>       statsLookup) =>
    IsTargetExists(selectLookup)
 && IsTargetOutOfRange(locTransLookup, statsLookup); // Out range

  public bool HaveTargetInRange(
    in ComponentLookup<Selectable>     selectLookup
  , in ComponentLookup<LocalTransform> locTransLookup
  , in BufferLookup<StatsBuffer>       statsLookup) =>
    IsTargetExists(selectLookup)
 && !IsTargetOutOfRange(locTransLookup, statsLookup); // In range

  public bool SoCloseToTarget(
    in ComponentLookup<Selectable>     selectLookup
  , in ComponentLookup<LocalTransform> locTransLookup
  , in BufferLookup<StatsBuffer>       statsLookup)
    =>
      _Stats[StatsId.UnitRadius].value
    + statsLookup[_AimedTargetData.ValueRO.target][StatsId.UnitRadius].value
    + CLOSE_TO_TARGET_TOLERANCE
    > math.length((
          _LocTrans.ValueRO.Position
        - locTransLookup[_AimedTargetData.ValueRO.target].Position)
        .WithoutY());
}