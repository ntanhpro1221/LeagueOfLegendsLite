using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct SyncHybridModelClientSystem : ISystem {
    public void OnUpdate(ref SystemState state) {
        state.CompleteDependency();

        UpdateModel(ref state);
        SyncSkillPreview_Turret(ref state);
        SyncSkillPreview_OwnChamp(ref state);
    }

    private void UpdateModel(ref SystemState state) {
        foreach (var data in SystemAPI.Query<UpdateModelAspect>())
            data.HybridData.ValueRO.UpdateModel(data);
    }

    private void SyncSkillPreview_Turret(ref SystemState state) {
        foreach (var data in SystemAPI
            .Query<UpdateSkillPreviewAspect>()
            .WithAll<TurretTag>())
            data.Update();
    }

    private void SyncSkillPreview_OwnChamp(ref SystemState state) {
        foreach (var data in SystemAPI
            .Query<UpdateSkillPreviewAspect>()
            .WithAll<
                ChampionTag
              , GhostOwnerIsLocal>())
            data.Update();
    }

    public readonly partial struct UpdateModelAspect : IAspect {
        public readonly RefRO<HybridModelData> HybridData;

        private readonly RefRO<SharedAnimData> _AnimData;
        private readonly RefRO<LocalTransform> _LocTrans;
        private readonly RefRO<RotationData>   _RotationData;
        private readonly RefRO<HighlightData>  _HighlightData;

        [Optional] private readonly EnabledRefRO<HighlightVisible> _HighlightTrigger;

        [Optional] public readonly RefRO<SkillPreviewData> PreviewData;

        public ref readonly float3         Pos  => ref _LocTrans.ValueRO.Position;
        public ref readonly floatXZ_Q3     Rot  => ref _RotationData.ValueRO.rotation;
        public ref readonly SharedAnimData Anim => ref _AnimData.ValueRO;

        public bool IsHighlighting =>
            _HighlightData.ValueRO.isHighlighted
         && _HighlightTrigger.ValueRO;
    }

    private readonly partial struct UpdateSkillPreviewAspect : IAspect {
        private readonly RefRO<HybridModelData>  _HybridData;
        private readonly RefRO<SkillPreviewData> _SkillPreviewData;

        public void Update() => _HybridData.ValueRO.skillPreviewRef.Value.Sync(_SkillPreviewData.ValueRO);
    }
}