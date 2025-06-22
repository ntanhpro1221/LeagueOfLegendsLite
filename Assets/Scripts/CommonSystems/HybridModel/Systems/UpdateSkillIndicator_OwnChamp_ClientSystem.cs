using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateBefore(typeof(SyncHybridModelClientSystem))]
public partial struct UpdateSkillIndicatorClientSystem : ISystem {
    private const float MAX_WARNING_RATIO = 2;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AllItemData>();
        state.RequireForUpdate<InputDirtyData>();
    }

    public void OnUpdate(ref SystemState state) {
        var  allItem          = SystemAPI.GetSingleton<AllItemData>();
        var  inputData        = SystemAPI.GetSingleton<InputDirtyData>();
        bool showNormalAttack = inputData.key_a.IsHolding();

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
            UpdateForTurret(ref state, entity, data.Pos, data.Team);

            var metadata = new IndicatorShower.Metadata();
            metadata.WithNormalAttack(showNormalAttack, data.Stats.data.AttackRange);

            var requestItem = Strum.SlotItem.First;
            for (; requestItem <= Strum.SlotItem.Last; ++requestItem)
                if ( // Is holding button
                    inputData.activableItem[requestItem].IsHolding()
                    // This item is available and activable
                 && data.ItemSlots.IsActivable(requestItem, allItem))
                    break;

            if (requestItem > Strum.SlotItem.Last) metadata.WithoutActivableItem();
            else
                metadata.WithActivableItem(requestItem, data.Level, data.ItemSlots.Slots[requestItem].level
                  , data.Input.inputForActivableItem, data.Input.curCondition);

            if (metadata.IsWithoutItem()) data.Indicator.UpdateShower(metadata);
            else data.Indicator.UpdateShower(metadata, ref data.ItemSlots.GetItemDataUnsafe(requestItem, allItem));
        }
    }

    private void UpdateForTurret(ref SystemState state, Entity ownChampEntity, float3 ownChampPos, TeamType ownChampTeam) {
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
              , RefRO<StatsData>
              , RefRO<TeamTypeData>
            >().WithAll<
                TurretTag
            >()) {
            if (team.ValueRO.team == ownChampTeam) continue;

            var metadata = new IndicatorShower.Metadata();
            metadata
                .WithNormalAttack(
                    showNormalAttack: MAX_WARNING_RATIO > GameHelpers.DistanceXZ(locTrans.ValueRO.Position, ownChampPos) / stats.ValueRO.data.AttackRange
                  , attackRange: stats.ValueRO.data.AttackRange)
                .WithTurretData(
                    ownChampPos: ownChampPos.Quantizate3()
                  , ownChampIsTarget: target.ValueRO.target == ownChampEntity
                  , ownerPos: locTrans.ValueRO.Position.Quantizate3());

            model.ValueRO.indicator.Value.UpdateShower(metadata);
        }
    }

    private readonly partial struct OwnChampAspect : IAspect {
        private readonly RefRO<HybridModelData> _Model;
        private readonly RefRO<LevelData>       _Level;
        private readonly RefRO<PlayerInputData> _Input;
        private readonly RefRO<LocalTransform>  _LocTrans;
        private readonly RefRO<TeamTypeData>    _Team;
        private readonly RefRO<StatsData>       _Stats;

        public readonly ItemSlotsAspectRO ItemSlots;

        public ref readonly StatsData Stats => ref _Stats.ValueRO;

        public ref readonly int Level => ref _Level.ValueRO.curLevel;

        public IndicatorShower Indicator => _Model.ValueRO.indicator.Value;

        public ref readonly PlayerInputData Input => ref _Input.ValueRO;
        public ref readonly float3          Pos   => ref _LocTrans.ValueRO.Position;
        public ref readonly TeamType        Team  => ref _Team.ValueRO.team;
    }
}