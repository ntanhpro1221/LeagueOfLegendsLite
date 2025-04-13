using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(BeforeMoveSystemGroup))]
public partial struct GetMoveDataFrom_AutoFollowTargetSystem : ISystem {
    [ReadOnly] private ComponentLookup<LocalTransform> locTransLookup;
    [ReadOnly] private ComponentLookup<TakeDamageSpot> takeDamageSpotLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
        locTransLookup = SystemAPI.GetComponentLookup<LocalTransform>(
            isReadOnly: true);
        takeDamageSpotLookup = SystemAPI.GetComponentLookup<TakeDamageSpot>(
            isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        locTransLookup.Update(ref state);
        takeDamageSpotLookup.Update(ref state);

        state.Dependency = new Job {
            locTransLookup       = locTransLookup
          , takeDamageSpotLookup = takeDamageSpotLookup
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(AutoFollowTarget))]
    [WithNone(
        typeof(NetworkDestroyedTag))]
    [BurstCompile]
    public partial struct Job : IJobEntity {
        [ReadOnly] public ComponentLookup<LocalTransform> locTransLookup;
        [ReadOnly] public ComponentLookup<TakeDamageSpot> takeDamageSpotLookup;

        [BurstCompile]
        public void Execute(in AimedTargetData target, ref MoveData moveData) {
            float3 takeDamageSpot = float3.zero;
            if (takeDamageSpotLookup.HasComponent(target.target))
                takeDamageSpot = takeDamageSpotLookup[target.target].spot;

            moveData.MoveTo(locTransLookup[target.target]
                .TransformPoint(takeDamageSpot)
                .Quantizate3());
        }
    }
}