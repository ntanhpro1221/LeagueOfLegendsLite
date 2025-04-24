using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateBefore(typeof(SyncHybridModelClientSystem))]
public partial struct UpdateSkillPreviewClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EnumIndexData>();
        state.RequireForUpdate<InputDirtyData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        ref var statsId       = ref SystemAPI.GetSingleton<EnumIndexData>().StatsType;
        var     attackRangeId = statsId[StatsType.AttackRange];
        var     unitRadiusId  = statsId[StatsType.UnitRadius];

        // Still loop through all own champions (it should just exist one)
        foreach (var (
                stats
              , skillPreview
              , locTrans
              , entity)
            in SystemAPI
                .Query<
                    DynamicBuffer<StatsBuffer>
                  , RefRW<SkillPreviewData>
                  , RefRO<LocalTransform>>()
                .WithAll<
                    ChampionTag
                  , GhostOwnerIsLocal>()
                .WithEntityAccess()) {

            // Update skill preview
            state.Dependency = new UpdateTurretJob {
                ownChampEntity   = entity
              , ownChampLocTrans = locTrans.ValueRO
              , ownChampRadius   = stats[unitRadiusId].value
              , attackRangeId    = attackRangeId
            }.ScheduleParallel(state.Dependency);

            // Update own champ preview
            UpdateOwnChamp(
                ref skillPreview.ValueRW
              , SystemAPI.GetSingleton<InputDirtyData>().a_key
              , stats[attackRangeId].value);
        }
    }

    [BurstCompile]
    public void UpdateOwnChamp(ref SkillPreviewData data, InputDirtyData.ButtonState aKey, float_Q3 attackRange) {
        data.type = aKey.IsHolding()
            ? SkillPreviewType.NormalAttack
            : SkillPreviewType.None;

        data.color = SkillPreviewColor.Blue;

        data.scale = 2 * new float2(attackRange);
    }

    [WithAll(typeof(TurretTag))]
    [WithPresent(typeof(DeadState))]
    [BurstCompile]
    public partial struct UpdateTurretJob : IJobEntity {
        public Entity         ownChampEntity;
        public LocalTransform ownChampLocTrans;
        public float_Q3       ownChampRadius;
        public int            attackRangeId;

        [BurstCompile]
        public void Execute(
            ref SkillPreviewData          data
          , AimedTargetAspectRO           target
          , in LocalTransform             locTrans
          , in DynamicBuffer<StatsBuffer> stats
          , EnabledRefRO<DeadState>       isDead) {

            // Reset first
            data.type = SkillPreviewType.None;

            // Dead => hide
            if (isDead.ValueRO) return;

            // Your champ is this turret's target
            if (target.Target == ownChampEntity)
                data.color = SkillPreviewColor.Red;

            // Your champ is near this turret (1.5 x range)
            else if (false == GameHelpers.IsTargetOutOfRange(
                ownChampLocTrans.Position
              , locTrans.Position
              , stats[attackRangeId].value * 1.5f
              , ownChampRadius))
                data.color = SkillPreviewColor.Orange;

            // Nothing happen
            else return;

            data.type  = SkillPreviewType.NormalAttack;
            data.scale = 2 * new float2(stats[attackRangeId].value);
        }
    }
} 