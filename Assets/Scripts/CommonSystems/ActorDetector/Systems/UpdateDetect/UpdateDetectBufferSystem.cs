using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

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

            AppendToBuffer_AllPartial(detector.holder, target);
        }
    }
}