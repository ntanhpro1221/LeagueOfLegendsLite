using Pathfinding;
using Pathfinding.ECS;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(BeforeFollowerEntityCalculateSystemGroup))]
public partial struct GetMoveDataFrom_AutoFollowTarget_FollowerEntitySystem : ISystem {
    [ReadOnly] private ComponentLookup<LocalTransform> locTransLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        locTransLookup = SystemAPI.GetComponentLookup<LocalTransform>(
            isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        locTransLookup.Update(ref state);

        state.Dependency = new Job {
            locTransLookup = locTransLookup
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(AutoFollowTarget_FollowerEntity))]
    [BurstCompile]
    public partial struct Job : IJobEntity {
        [ReadOnly] public ComponentLookup<LocalTransform> locTransLookup;

        [BurstCompile]
        public void Execute(
            in  AimedTargetData  target
          , ref DestinationPoint desSetter) {
            desSetter.destination = locTransLookup[target.target].Position;
        }
    }
}