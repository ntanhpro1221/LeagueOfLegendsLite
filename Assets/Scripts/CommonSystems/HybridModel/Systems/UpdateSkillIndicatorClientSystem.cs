using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateBefore(typeof(SyncHybridModelClientSystem))]
public partial struct UpdateSkillIndicatorClientSystem : ISystem {
    private const float MAX_WARNING_RATIO = 2;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EnumIndexData>();
        state.RequireForUpdate<InputDirtyData>();
        state.RequireForUpdate<InputDirtyData.ActivableItemBuffer>();
    }

    public void OnUpdate(ref SystemState state) {
        int  rangeId          = SystemAPI.GetSingleton<EnumIndexData>().StatsType[StatsType.AttackRange];
        bool showNormalAttack = SystemAPI.GetSingleton<InputDirtyData>().key_a.IsHolding();
        var  dirtyBuffer      = SystemAPI.GetSingletonBuffer<InputDirtyData.ActivableItemBuffer>(isReadOnly: true);
        var  itemKey          = default(PlayerTrigger.Item);
        for (; itemKey < PlayerTrigger.Item.COUNT; ++itemKey)
            if (dirtyBuffer[(int)itemKey].key is InputDirtyData.ButtonState.Down or InputDirtyData.ButtonState.Hold)
                break;

        // Still loop through all own champions (it should just exist one)
        foreach (var (
            data
          , entity
            ) in SystemAPI
            .Query<
                OwnChampAspect
            >().WithAll<
                ChampionTag
              , GhostOwnerIsLocal
            >().WithNone<
                DummyTag
            >().WithEntityAccess()) {
            UpdateForTurret(ref state, rangeId, entity, data.Pos, data.Team);

            var metadata = new IndicatorShower.Metadata();
            metadata.WithNormalAttack(showNormalAttack, data.Stats[rangeId].value);
            if (itemKey == PlayerTrigger.Item.COUNT) metadata.WithoutActivableItem();
            else
                metadata.WithActivableItem(itemKey, data.Level, data.ItemsDynamic[(int)itemKey].level
                  , data.Input.inputForActivableItem, data.Input.curCondition);

            data.Indicator.UpdateShower(metadata, ref data.ItemsStatic[
                itemKey == PlayerTrigger.Item.COUNT
                    ? PlayerTrigger.Item.Skill_Passive
                    : itemKey]);
        }
    }

    private void UpdateForTurret(ref SystemState state, int rangeId, Entity ownChampEntity, float3 ownChampPos, TeamType ownChampTeam) {
        foreach (var (
            locTrans
          , target
          , model
          , stats
          , team
            ) in SystemAPI
            .Query<
                RefRO<LocalTransform>
              , RefRO<AimedTargetData>
              , RefRO<HybridModelData>
              , DynamicBuffer<StatsBuffer>
              , RefRO<TeamTypeData>
            >().WithAll<
                TurretTag
            >()) {
            if (team.ValueRO.team == ownChampTeam) continue;

            var metadata = new IndicatorShower.Metadata();
            metadata
                .WithNormalAttack(
                    showNormalAttack: MAX_WARNING_RATIO > GameHelpers.DistanceXZ(locTrans.ValueRO.Position, ownChampPos) / stats[rangeId].value
                  , attackRange: stats[rangeId].value)
                .WithTurretData(
                    ownChampPos: ownChampPos.Quantizate3()
                  , ownChampIsTarget: target.ValueRO.target == ownChampEntity
                  , ownerPos: locTrans.ValueRO.Position.Quantizate3());

            model.ValueRO.indicator.Value.UpdateShower(metadata);
        }
    }

    private readonly partial struct OwnChampAspect : IAspect {
        private readonly RefRO<HybridModelData>      _Model;
        private readonly RefRO<LevelData>            _Level;
        private readonly RefRO<PlayerInputData>      _Input;
        private readonly RefRO<AllActivableItemData> _ItemsStatic;
        private readonly RefRO<LocalTransform>       _LocTrans;
        private readonly RefRO<TeamTypeData>         _Team;

        [ReadOnly] public readonly DynamicBuffer<StatsBuffer>              Stats;
        [ReadOnly] public readonly DynamicBuffer<ActivableItemBonusBuffer> ItemsDynamic;

        public int             Level     => _Level.ValueRO.curLevel;
        public IndicatorShower Indicator => _Model.ValueRO.indicator.Value;

        public ref readonly PlayerInputData      Input       => ref _Input.ValueRO;
        public ref readonly AllActivableItemData ItemsStatic => ref _ItemsStatic.ValueRO;
        public ref readonly float3               Pos         => ref _LocTrans.ValueRO.Position;
        public ref readonly TeamType             Team        => ref _Team.ValueRO.team;
    }
}