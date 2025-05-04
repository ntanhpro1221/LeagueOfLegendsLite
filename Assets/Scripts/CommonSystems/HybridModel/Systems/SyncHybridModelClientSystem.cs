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
        SyncAnim_PosHighlight(ref state);
        SyncSkillPreview_Turret(ref state);
        SyncSkillPreview_OwnChamp(ref state);
    }

    private void SyncAnim_PosHighlight(ref SystemState state) {
        foreach (var (
                hybridData
              , animData
              , locTrans
              , highlightData
              , highlightVisible
              , entity)
            in SystemAPI.Query<
                    RefRO<HybridModelData>
                  , RefRW<SharedAnimData>
                  , RefRO<LocalTransform>
                  , RefRO<HighlightData>
                  , EnabledRefRO<HighlightVisible>>()
                .WithPresent<HighlightVisible>()
                .WithNone<NeedInitTag>()
                .WithEntityAccess()) {
            var trans    = hybridData.ValueRO.transformRef.Value;
            var animCtrl = hybridData.ValueRO.animCtrlRef.Value;
            var outline  = hybridData.ValueRO.outlineRef.Value;

            // POSITION
            if (locTrans.ValueRO.Position.IsNaN())
                Debug.LogWarning($"NGDtuanh: {state.WorldName()} position of entity({entity.Index}) is NaN => {locTrans.ValueRO.Position}");
            else trans.position = locTrans.ValueRO.Position;
            trans.rotation = locTrans.ValueRO.Rotation;

            // ANIMATION
            animCtrl.SyncAnim(
                animData.ValueRO.curAnim
              , ref animData.ValueRW.isNeedRestart
              , animData.ValueRO.hardCutAnim);

            // HIGHLIGHT
            bool isHighlightNow =
                highlightData.ValueRO.isHighlighted
             && highlightVisible.ValueRO;
            if (isHighlightNow != outline.enabled)
                outline.enabled = isHighlightNow;
        }
    }

    private void SyncSkillPreview_Turret(ref SystemState state) {
        foreach (var data in SystemAPI
            .Query<SyncSkillPreviewAspect>()
            .WithAll<TurretTag>())
            data.Sync();
    }

    private void SyncSkillPreview_OwnChamp(ref SystemState state) {
        foreach (var data in SystemAPI
            .Query<SyncSkillPreviewAspect>()
            .WithAll<
                ChampionTag
              , GhostOwnerIsLocal>())
            data.Sync();
    }

    private readonly partial struct SyncSkillPreviewAspect : IAspect {
        private readonly RefRO<HybridModelData>  _HybridData;
        private readonly RefRO<SkillPreviewData> _SkillPreviewData;

        public void Sync() {
            _HybridData.ValueRO.skillPreviewRef.Value.Sync(
                _SkillPreviewData.ValueRO.type
              , _SkillPreviewData.ValueRO.color
              , _SkillPreviewData.ValueRO.scale);
        }
    }
}