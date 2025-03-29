using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct ApplyTransformAndAnimHybridModelClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        using EntityQueryBuilder queryBuilder = new(Allocator.Temp);
        state.RequireForUpdate(queryBuilder
            .WithAll<
                HybridModelData
              , AnimData
              , LocalToWorld>()
            .Build(ref state));
    }

    public void OnUpdate(ref SystemState state) {
        using EntityCommandBuffer ecb = new(Allocator.Temp);

        foreach (var (
                hybridData
              , animData
              , localToWorld)
            in SystemAPI.Query<
                RefRO<HybridModelData>
              , RefRO<AnimData>
              , RefRO<LocalToWorld>>()) {
            var trans    = hybridData.ValueRO.transformRef.Value;
            var animCtrl = hybridData.ValueRO.animCtrlRef.Value;

            trans.position = localToWorld.ValueRO.Position;
            trans.rotation = localToWorld.ValueRO.Rotation;

            animCtrl.SyncAnim(animData.ValueRO.curAnim);
        }

        ecb.Playback(state.EntityManager);
    }
}