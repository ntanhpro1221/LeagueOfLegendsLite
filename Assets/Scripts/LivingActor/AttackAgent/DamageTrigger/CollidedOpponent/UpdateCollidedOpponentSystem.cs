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
        _UpdateCollidedJob = new();
        _UpdateCollidedJob.Init(
            SystemAPI.GetComponentLookup<TeamTypeData>(true)
          , SystemAPI.GetBufferLookup<CollidedOpponentBuffer>()
          , SystemAPI.GetBufferLookup<IncomingDamageBuffer>(true));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        _UpdateCollidedJob.Update(ref state);
        state.Dependency = _UpdateCollidedJob.Schedule(
            SystemAPI.GetSingleton<SimulationSingleton>()
          , state.Dependency);
    }

    [BurstCompile]
    public struct UpdateCollidedOpponentJob : ITriggerEventsJob {
        [ReadOnly] private ComponentLookup<TeamTypeData>      _TeamTypeLookup;
        [ReadOnly] private BufferLookup<IncomingDamageBuffer> _IncomingDamageLookup;

        private BufferLookup<CollidedOpponentBuffer> _CollidedOpponentLookup;

        [BurstCompile]
        public void Init(
            ComponentLookup<TeamTypeData>        teamTypeLookup
          , BufferLookup<CollidedOpponentBuffer> collidedOpponentLookup
          , BufferLookup<IncomingDamageBuffer>   incomingDamageLookup) {
            _TeamTypeLookup         = teamTypeLookup;
            _CollidedOpponentLookup = collidedOpponentLookup;
            _IncomingDamageLookup   = incomingDamageLookup;
        }

        [BurstCompile]
        public void Update(ref SystemState state) {
            _TeamTypeLookup.Update(ref state);
            _CollidedOpponentLookup.Update(ref state);
            _IncomingDamageLookup.Update(ref state);
        }

        [BurstCompile]
        public void Execute(TriggerEvent triggerEvent) {
            Entity alice = triggerEvent.EntityA;
            Entity bob   = triggerEvent.EntityB;

            if (!IsOpponent(alice, bob)) return;

            TryAppendToCollidedBuffer(alice, bob);
            TryAppendToCollidedBuffer(bob,   alice);
        }

        [BurstCompile]
        private void TryAppendToCollidedBuffer(
            in Entity alice
          , in Entity collideWith) {
            if (!_CollidedOpponentLookup.HasBuffer(alice)) return;            // alice is not damager
            if (!_IncomingDamageLookup.HasBuffer(collideWith)) return;        // not a damageable guy
            if (_CollidedOpponentLookup[alice].Contains(collideWith)) return; // already collide

            _CollidedOpponentLookup[alice].Add(new() {
                entity = collideWith
            });
        }

        [BurstCompile]
        private bool IsOpponent(
            in Entity alice
          , in Entity bob) =>
            _TeamTypeLookup.HasComponent(alice)
         && _TeamTypeLookup.HasComponent(bob)
         && _TeamTypeLookup[alice].team != _TeamTypeLookup[bob].team;
    }
}