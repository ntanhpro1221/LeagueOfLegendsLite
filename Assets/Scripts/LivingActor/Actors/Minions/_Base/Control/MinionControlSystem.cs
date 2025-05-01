using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// Just follow <see cref="MinionFixedPathBuffer"/> now
/// </summary>
[UpdateInGroup(typeof(ActorAIControlSystemGroup))]
public partial struct MinionControlSystem : ISystem {
    // private const float REACH_PATH_DIS_TOLERANCE_SQR = 100f;
    //
    // [BurstCompile]
    // public void OnUpdate(ref SystemState state) {
    //     state.Dependency = new Job()
    //         .ScheduleParallel(state.Dependency);
    // }
    //
    // [WithAll(
    //     typeof(Simulate)
    //   , typeof(MinionTag))]
    // [WithNone(typeof(NeedInitTag))]
    // [BurstCompile]
    // private partial struct Job : IJobEntity {
    //     [BurstCompile]
    //     public void Execute(
    //         ref MinionControlData                    controlData
    //       , ref DynamicBuffer<MinionFixedPathBuffer> pathBuffer
    //       , MoveRequesterAspect                      moveRequester
    //       , in LocalTransform                        locTrans) {
    //
    //         if (!pathBuffer.Empty()
    //          && REACH_PATH_DIS_TOLERANCE_SQR > GameHelpers.DistanceXZ_Sqr(locTrans.Position, pathBuffer.FrontRO().pos)) {
    //             pathBuffer.PopFront();
    //             if (!pathBuffer.Empty())
    //                 moveRequester.MoveSmartTo(pathBuffer.FrontRO().pos);
    //         }
    //     }
    // }
}