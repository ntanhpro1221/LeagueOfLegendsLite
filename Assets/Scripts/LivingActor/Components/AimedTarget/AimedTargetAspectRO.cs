using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct AimedTargetAspectRO : IAspect {
  public const int CLOSE_TO_TARGET_TOLERANCE = 5;

  private readonly RefRO<LocalTransform>  _LocTrans;
  private readonly RefRO<AimedTargetData> _AimedTargetData;

  private readonly RefRO<StatsData> _Stats;

  public ref readonly Entity Target => ref _AimedTargetData.ValueRO.target;

  public bool IsTargetExists(in ComponentLookup<Selectable> selectLookup) =>
    GameHelpers.IsTargetExists(
      _AimedTargetData.ValueRO.target
    , selectLookup);

  public bool IsTargetOutOfRange(
    in ComponentLookup<LocalTransform> locTransLookup
  , in ComponentLookup<StatsData>      statsLookup) =>
    GameHelpers.IsTargetOutOfRange(
      _LocTrans.ValueRO.Position
    , locTransLookup[_AimedTargetData.ValueRO.target].Position
    , _Stats.ValueRO.data.AttackRange
    , statsLookup[_AimedTargetData.ValueRO.target].data.UnitRadius);

  public bool NeedMoveToTarget(
    in ComponentLookup<Selectable>     selectLookup
  , in ComponentLookup<LocalTransform> locTransLookup
  , in ComponentLookup<StatsData>      statsLookup) =>
    IsTargetExists(selectLookup)
 && IsTargetOutOfRange(locTransLookup, statsLookup); // Out range

  public bool HaveTargetInRange(
    in ComponentLookup<Selectable>     selectLookup
  , in ComponentLookup<LocalTransform> locTransLookup
  , in ComponentLookup<StatsData>      statsLookup) =>
    IsTargetExists(selectLookup)
 && !IsTargetOutOfRange(locTransLookup, statsLookup); // In range

  public bool SoCloseToTarget(
    in ComponentLookup<Selectable>     selectLookup
  , in ComponentLookup<LocalTransform> locTransLookup
  , in ComponentLookup<StatsData>      statsLookup)
    =>
      _Stats.ValueRO.data.UnitRadius
    + statsLookup[_AimedTargetData.ValueRO.target].data.UnitRadius
    + CLOSE_TO_TARGET_TOLERANCE
    > math.length((
          _LocTrans.ValueRO.Position
        - locTransLookup[_AimedTargetData.ValueRO.target].Position)
        .WithoutY());
}