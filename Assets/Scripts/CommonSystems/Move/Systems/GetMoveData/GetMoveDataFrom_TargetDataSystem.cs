using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;



/// <summary>
///
/// 
/// NOT WORK ANYMORE, AimedTargetData is for attacking<br/>
/// You should create something like FollowTargetData and create another system to auto follow this tar get<br/>
/// 
/// 
/// </summary>





[UpdateInGroup(typeof(BeforeMoveSystemGroup))]
public partial struct GetMoveDataFrom_TargetDataSystem : ISystem {
    // private EntityStorageInfoLookup              entityLookup;
    // private ComponentLookup<LocalTransform>      localTransLookup;
    // private ComponentLookup<Parent>              parentLookup;
    // private ComponentLookup<PostTransformMatrix> scaleLookup;
    //
    // [BurstCompile]
    // public void OnCreate(ref SystemState state) {
    //     entityLookup         = SystemAPI.GetEntityStorageInfoLookup();
    //     localTransLookup = SystemAPI.GetComponentLookup<LocalTransform>();
    //     parentLookup         = SystemAPI.GetComponentLookup<Parent>();
    //     scaleLookup          = SystemAPI.GetComponentLookup<PostTransformMatrix>();
    // }
    //
    // [BurstCompile]
    // public void OnUpdate(ref SystemState state) {
    //     entityLookup.Update(ref state);
    //     localTransLookup.Update(ref state);
    //     parentLookup.Update(ref state);
    //     scaleLookup.Update(ref state);
    //
    //     foreach (var (
    //             moveData
    //           , targetData
    //           , parent)
    //         in SystemAPI.Query<
    //                 RefRW<MoveData>
    //               , AimedTargetAspectRO
    //               , RefRO<Parent>>()
    //             .WithAll<Simulate>()
    //             .WithNone<NetworkDestroyedTag>()) {
    //         if (!targetData.IsTargetExists(entityLookup)) return;
    //
    //         GetWorldTransformMatrix(targetData.Target, out var targetWorldMatrix);
    //         var targetWorldPos = targetWorldMatrix.TransformPoint(float3.zero);
    //         GetWorldTransformMatrix(parent.ValueRO.Value, out var thisWorldMatrix);
    //
    //         moveData.ValueRW.targetLocalPos = (float3_Q3)thisWorldMatrix.InverseTransformPoint(targetWorldPos);
    //     }
    //
    //     foreach (var (
    //             moveData
    //           , targetData)
    //         in SystemAPI.Query<
    //                 RefRW<MoveData>
    //               , AimedTargetAspectRO>()
    //             .WithAll<Simulate>()
    //             .WithNone<NetworkDestroyedTag, Parent>()) {
    //         if (!targetData.IsTargetExists(entityLookup)) return;
    //         
    //         GetWorldTransformMatrix(targetData.Target, out var targetWorldMatrix);
    //         var targetWorldPos = targetWorldMatrix.TransformPoint(float3.zero);
    //
    //         moveData.ValueRW.targetLocalPos = (float3_Q3)targetWorldPos;
    //     }
    // }
    //
    // private void GetWorldTransformMatrix(in Entity entity, out float4x4 result)
    //     => TransformHelpers.ComputeWorldTransformMatrix(
    //         entity
    //       , out result
    //       , ref localTransLookup
    //       , ref parentLookup
    //       , ref scaleLookup);
}