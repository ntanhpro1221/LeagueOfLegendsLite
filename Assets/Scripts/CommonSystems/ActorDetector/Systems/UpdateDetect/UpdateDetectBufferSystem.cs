using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

/// <summary>
/// To add more detect agent, look at:
/// - Create partial for it
/// - Call it in partial center
/// - Add new corresponding data in partial center
/// </summary>
[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
[BurstCompile]
public partial struct UpdateDetectedActorSystem : ISystem {
    private MainJob mainJob;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<SimulationSingleton>();

        InitBuffer_AllPartial(ref state);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        ScheduleClearBuffer_AllPartial(ref state);

        state.CompleteDependency();

        UpdateData_AllPartial(ref state);

        state.Dependency = mainJob.Schedule(
            SystemAPI.GetSingleton<SimulationSingleton>()
          , state.Dependency);
    }

    [BurstCompile]
    private partial struct MainJob : ITriggerEventsJob {
        public Data data;

        [BurstCompile]
        public void Execute(TriggerEvent triggerEvent) {
            var alice = triggerEvent.EntityA;
            var bob   = triggerEvent.EntityB;

            TryAppendToHolder(alice, bob);
            TryAppendToHolder(bob,   alice);
        }

        [BurstCompile]
        private void TryAppendToHolder(in Entity detectorEntity, in Entity target) {
            if (!data.actorDetectorLookup.TryGetComponent(detectorEntity, out var detector)
             || detector.holder == target) // Not include itself
                return;

            if (!data.filterLookup.EntityExists(detector.holder)
             || !data.filterLookup.EntityExists(target)) {
                // Debug.LogWarning($"NGDtuanh: holder or target of detector doesn't exist (may be relative to predicted spawn ghost)");
                return;
            }

            // Team filter
            switch (data.filterLookup[detector.holder].teamFilter) {
                case ActorDetectFilter.TeamFilter.Opponent:
                    if (!data.teamLookup[detector.holder].IsRedBlue(data.teamLookup[target]))
                        return;
                    break;
                case ActorDetectFilter.TeamFilter.All:
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            AppendToBuffer_AllPartial(detector.holder, target);
        }
    }
}