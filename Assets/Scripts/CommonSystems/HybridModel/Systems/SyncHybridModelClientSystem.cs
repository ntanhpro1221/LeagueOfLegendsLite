using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct SyncHybridModelClientSystem : ISystem {
    public void OnUpdate(ref SystemState state) {
        state.CompleteDependency();

        foreach (var data in SystemAPI.Query<UpdateModelAspect>())
            data.HybridData.ValueRO.UpdateModel(data);
    }

    public readonly partial struct UpdateModelAspect : IAspect {
        public readonly RefRO<HybridModelData> HybridData;
 
        private readonly RefRO<SharedAnimData> _AnimData;
        private readonly RefRO<LocalTransform> _LocTrans;
        private readonly RefRO<RotationData>   _RotationData;
        private readonly RefRO<HighlightData>  _HighlightData;

        [Optional] private readonly EnabledRefRO<HighlightVisible> _HighlightTrigger;

        public ref readonly float3         Pos  => ref _LocTrans.ValueRO.Position;
        public ref readonly floatXZ_Q3     Rot  => ref _RotationData.ValueRO.rotation;
        public ref readonly SharedAnimData Anim => ref _AnimData.ValueRO;

        public bool IsHighlighting =>
            _HighlightData.ValueRO.isHighlighted
         && _HighlightTrigger.ValueRO;
    }
}