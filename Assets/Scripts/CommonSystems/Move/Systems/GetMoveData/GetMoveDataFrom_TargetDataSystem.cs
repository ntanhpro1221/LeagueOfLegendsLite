using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(BeforeMoveSystemGroup))]
public partial struct GetMoveDataFrom_TargetDataSystem : ISystem {
    private ComponentLookup<LocalTransform>      localTransformLookup;
    private ComponentLookup<Parent>              parentLookup;
    private ComponentLookup<PostTransformMatrix> scaleLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        localTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
        parentLookup         = SystemAPI.GetComponentLookup<Parent>();
        scaleLookup          = SystemAPI.GetComponentLookup<PostTransformMatrix>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        localTransformLookup.Update(ref state);
        parentLookup.Update(ref state);
        scaleLookup.Update(ref state);

        foreach (var (
                moveData
              , targetData
              , parent)
            in SystemAPI.Query<
                    RefRW<MoveData>
                  , RefRO<DamageTargetData>
                  , RefRO<Parent>>()
                .WithAll<Simulate>()
                .WithNone<NetworkDestroyedTag>()) {
            GetWorldTransformMatrix(targetData.ValueRO.target, out var targetWorldMatrix);
            var targetWorldPos = targetWorldMatrix.TransformPoint(float3.zero);
            GetWorldTransformMatrix(parent.ValueRO.Value, out var thisWorldMatrix);

            moveData.ValueRW.targetLocalPos = (float3_Q3)thisWorldMatrix.InverseTransformPoint(targetWorldPos);
        }

        foreach (var (
                moveData
              , targetData)
            in SystemAPI.Query<
                    RefRW<MoveData>
                  , RefRO<DamageTargetData>>()
                .WithAll<Simulate>()
                .WithNone<NetworkDestroyedTag, Parent>()) {
            GetWorldTransformMatrix(targetData.ValueRO.target, out var targetWorldMatrix);
            var targetWorldPos = targetWorldMatrix.TransformPoint(float3.zero);

            moveData.ValueRW.targetLocalPos = (float3_Q3)targetWorldPos;
        }
    }

    private void GetWorldTransformMatrix(in Entity entity, out float4x4 result)
        => TransformHelpers.ComputeWorldTransformMatrix(
            entity
          , out result
          , ref localTransformLookup
          , ref parentLookup
          , ref scaleLookup);
}