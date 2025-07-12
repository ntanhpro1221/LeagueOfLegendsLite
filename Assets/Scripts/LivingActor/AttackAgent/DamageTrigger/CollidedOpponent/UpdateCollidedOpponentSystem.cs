using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct UpdateCollidedOpponentSystem : ISystem {
    private UpdateCollidedOpponentJob _UpdateCollidedJob;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<SimulationSingleton>();
        _UpdateCollidedJob = new UpdateCollidedOpponentJob();
        _UpdateCollidedJob.Init(
            SystemAPI.GetComponentLookup<TeamTypeData>(true)
          , SystemAPI.GetBufferLookup<CollidedOpponentBuffer>());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        _UpdateCollidedJob.Update(ref state);
        state.Dependency = _UpdateCollidedJob.Schedule(
            SystemAPI.GetSingleton<SimulationSingleton>()
          , state.Dependency);
    }

    [BurstCompile]
    private struct UpdateCollidedOpponentJob : ITriggerEventsJob {
        [ReadOnly] private ComponentLookup<TeamTypeData> _TeamTypeLookup;

        private BufferLookup<CollidedOpponentBuffer> _CollidedOpponentLookup;

        [BurstCompile]
        public void Init(
            ComponentLookup<TeamTypeData>        teamTypeLookup
          , BufferLookup<CollidedOpponentBuffer> collidedOpponentLookup) {
            _TeamTypeLookup         = teamTypeLookup;
            _CollidedOpponentLookup = collidedOpponentLookup;
        }

        [BurstCompile]
        public void Update(ref SystemState state) {
            _TeamTypeLookup.Update(ref state);
            _CollidedOpponentLookup.Update(ref state);
        }

        [BurstCompile]
        public void Execute(TriggerEvent triggerEvent) {
            var alice = triggerEvent.EntityA;
            var bob   = triggerEvent.EntityB;

            // Filter out not opponent pair (don't have team or in the same team).
            if (!_TeamTypeLookup.TryGetComponent(alice, out var teamAlice)
             || !_TeamTypeLookup.TryGetComponent(bob,   out var teamBob)
             || teamAlice.IsSameTeam(teamBob))
                return;

            TryAppendToBuffer(alice, bob);
            TryAppendToBuffer(bob,   alice);
        }

        [BurstCompile]
        private void TryAppendToBuffer(
            in Entity alice
          , in Entity collideWith) {
            if (!_CollidedOpponentLookup.TryGetBuffer(alice, out var buffer)) return; // alice is not damager
            if (buffer.Contains(collideWith)) return;                                 // already collided

            buffer.Add(new CollidedOpponentBuffer { entity = collideWith });
        }
    }
}