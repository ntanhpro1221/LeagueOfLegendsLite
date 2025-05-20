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
        SyncAnim_TransHighlight(ref state);
        SyncSkillPreview_Turret(ref state);
        SyncSkillPreview_OwnChamp(ref state);
    }

    private void SyncAnim_TransHighlight(ref SystemState state) {
        foreach (var (
                hybridData
              , animData
              , locTrans
                , rotationData
              , highlightData
              , highlightVisible)
            in SystemAPI.Query<
                    RefRO<HybridModelData>
                  , RefRW<SharedAnimData>
                  , RefRO<LocalTransform>
                  , RefRO<RotationData>
                  , RefRO<HighlightData>
                  , EnabledRefRO<HighlightVisible>>()
                .WithPresent<HighlightVisible>()
                .WithNone<NeedInitTag>()) {
            var trans    = hybridData.ValueRO.transformRef.Value;
            var animCtrl = hybridData.ValueRO.animCtrlRef.Value;
            var outline  = hybridData.ValueRO.outlineRef.Value;
            var rotation = hybridData.ValueRO.rotateRef.Value;

            // POSITION
            if (!locTrans.ValueRO.Position.IsAnyNaN()) trans.position = locTrans.ValueRO.Position;
            
            // ROTATION
            rotation.RotateTo(rotationData.ValueRO.rotation);

            // ANIMATION
            animCtrl.SyncAnim(
                animData.ValueRO.curAnim
              , animData.ValueRW.currentSessionToRestart
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