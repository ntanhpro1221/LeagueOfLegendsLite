using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
[UpdateAfter(typeof(CorrectMoveSystem))]
public partial struct SyncActorDetectorPositionWithItsHolderSystem : ISystem {
    [ReadOnly] private ComponentLookup<LocalTransform> locTransLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        locTransLookup = SystemAPI.GetComponentLookup<LocalTransform>(
            isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        locTransLookup.Update(ref state);

        state.Dependency = new GetHolderPositionJob {
            locTransLookup = locTransLookup
        }.ScheduleParallel(state.Dependency);

        state.Dependency = new UpdatePositionJob()
            .ScheduleParallel(state.Dependency);
    }


    [WithAll(typeof(Simulate))]
    [WithDisabled(typeof(DontFollowHolder))]
    [BurstCompile]
    private partial struct GetHolderPositionJob : IJobEntity {
        [ReadOnly] public ComponentLookup<LocalTransform> locTransLookup;

        /// <summary>
        /// Must validate <see cref="actorDetector"/> because ==> see: <see cref="ActorDetector.holder"/>
        /// </summary>
        [BurstCompile]
        public void Execute(ref ActorDetector actorDetector) {
            if (locTransLookup.TryGetComponent(actorDetector.holder, out var holder))
                actorDetector.tmpHolderPosition = holder.Position;
        }
    }

    [WithAll(typeof(Simulate))]
    [WithDisabled(typeof(DontFollowHolder))]
    [BurstCompile]
    private partial struct UpdatePositionJob : IJobEntity {
        [BurstCompile]
        public void Execute(ref LocalTransform locTrans, in ActorDetector actorDetector) {
            locTrans.Position = actorDetector.tmpHolderPosition;
        }
    }
}