using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateAfter(typeof(CameraFollowClientSystem))]
public partial struct SyncHybridHealthBarClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<RequireExpData>();
        state.RequireForUpdate<NetworkTime>();
    }

    public void OnUpdate(ref SystemState state) {
        using EntityCommandBuffer ecb = new(Allocator.Temp);

        var     cam            = Camera.main;
        var     requireExp     = SystemAPI.GetSingleton<RequireExpData>();
        var     curTick        = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

        foreach (var data in SystemAPI.Query<UpdateAspect>()) {
            var uiUpdateData = data.HealthBarUpdateAspect.GenerateUpdateData(requireExp);

            data.DynamicUI.Update(
                uiUpdateData
              , data.LocTrans
              , data.HealthBarVisible.ValueRO
              , cam);

            if (data.HaveStickyUI) {
                data.StickyUI.Update(uiUpdateData, data.DeadTriggerUI.ValueRO
                  , curTick, data.DeadData.ValueRO, data.DeadState);
            }
        }

        ecb.Playback(state.EntityManager);
    }

    private readonly partial struct UpdateAspect : IAspect {
        private readonly RefRO<HybridHealthBarData> _HybridData;
        private readonly RefRO<LocalTransform>      _LocTrans;

        public readonly HealthBarUpdateAspect HealthBarUpdateAspect;

        public readonly RefRO<DeadStateData> DeadData;
        
        [Optional] public readonly EnabledRefRO<HybridHealthBarVisible> HealthBarVisible;
        [Optional] public readonly EnabledRefRO<DeadState>              DeadState;
        [Optional] public readonly RefRO<DeadTriggerForUIData>          DeadTriggerUI;

        public bool HaveStickyUI => _HybridData.ValueRO.sticky.initialized;

        public ref readonly HybridHealthBarData.Dynamic DynamicUI => ref _HybridData.ValueRO.dynamic;
        public ref readonly HybridHealthBarData.Sticky  StickyUI  => ref _HybridData.ValueRO.sticky;
        public ref readonly LocalTransform              LocTrans  => ref _LocTrans.ValueRO;
    }
}