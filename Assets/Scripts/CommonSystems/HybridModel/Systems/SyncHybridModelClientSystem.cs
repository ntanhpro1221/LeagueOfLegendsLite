using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct SyncHybridModelClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EnumIndexData>();
        state.RequireForUpdate<InputDirtyData>();
    }

    public void OnUpdate(ref SystemState state) {
        Sync_Anim_Pos_Highlight(ref state);
        Sync_AttackRangeShower(ref state);
    }

    private void Sync_Anim_Pos_Highlight(ref SystemState state) {
        foreach (var (
                hybridData
              , animData
              , locTrans
              , highlightData)
            in SystemAPI.Query<
                RefRO<HybridModelData>
              , RefRW<SharedAnimData>
              , RefRO<LocalTransform>
              , RefRO<HighlightData>>()) {
            var trans    = hybridData.ValueRO.transformRef.Value;
            var animCtrl = hybridData.ValueRO.animCtrlRef.Value;
            var outline  = hybridData.ValueRO.outlineRef.Value;

            trans.position = locTrans.ValueRO.Position;
            trans.rotation = locTrans.ValueRO.Rotation;

            animCtrl.SyncAnim(animData.ValueRO.curAnim, ref animData.ValueRW.isNeedRestart);

            if (highlightData.ValueRO.isHighlighted != outline.enabled)
                outline.enabled = highlightData.ValueRO.isHighlighted;
        }
    }

    private void Sync_AttackRangeShower(ref SystemState state) {
        var attackRangeId = SystemAPI.GetSingleton<EnumIndexData>().StatsType[StatsType.AttackRange];

        foreach (var (
                shower
              , stats)
            in SystemAPI
                .Query<
                    RefRO<HybridModelData>
                  , DynamicBuffer<StatsBuffer>>()
                .WithAll<
                    ChampionTag
                  , GhostOwnerIsLocal>())
            shower.ValueRO.attackRangeShowerRef.Value.SyncType(
                SystemAPI.GetSingleton<InputDirtyData>().a_key.IsHolding()
                    ? SkillPreviewType.NormalAttack
                    : SkillPreviewType.None
              , SkillPreviewColor.Blue
              , 2 * new Vector2(stats[attackRangeId].value, stats[attackRangeId].value));
    }
}