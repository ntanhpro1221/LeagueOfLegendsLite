using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Entities;

public static partial class TowerStateIdle {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct Exit : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (
                    filter
                  , health
                  , sharedState)
                in SystemAPI.Query<
                    StateFilterAspect
                  , HealthAspectRO
                  , ActorSharedStateAspect>()) {

                // DEAD STATE
                if (health.IsDead) // RUN OUT OF HEALTH
                    sharedState.SetDead();

                else continue;

                filter.MarkExitExecuted();
            }
        }
    }

    [UpdateInGroup(typeof(StateEnterSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct Enter : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (_, anim) in SystemAPI.Query<
                StateFilterAspect
              , SharedAnimAspect>()) {
                anim.SetAnim(SharedAnimKey.Idle);
            }
        }
    }
}

public static partial class TowerStateIdle {
    public partial struct Exit {
        public struct InheritTag : IStateInheritTag<TowerTag, IdleState> { }

        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<TowerTag, IdleState>.Base<InheritTag> {
            private readonly RefRO<TowerTag> _identity;
            private readonly RefRO<Simulate> _simulate;

            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<IdleState>         _curStateEnable;

            RefRO<TowerTag> IStateAspect<TowerTag, IdleState>.Identity => _identity;
            RefRO<Simulate> IStateAspect<TowerTag, IdleState>.Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitAspect<TowerTag, IdleState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<IdleState> IStateExitAspect<TowerTag, IdleState>.        CurStateEnable    => _curStateEnable;

            public void MarkExitExecuted() => _stateNotExitedYet.ValueRW = _curStateEnable.ValueRW = false;


            private readonly RefRO<InheritTag>                                   _inheritTag;
            RefRO<InheritTag> IStateInheritable<TowerTag, IdleState, InheritTag>.InheritTag => _inheritTag;
        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<TowerTag, IdleState> {
            private readonly RefRO<TowerTag>  _identity;
            private readonly RefRO<IdleState> _curState;
            private readonly RefRO<Simulate>  _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<TowerTag> IStateAspect<TowerTag, IdleState>. Identity => _identity;
            RefRO<IdleState> IStateAspect<TowerTag, IdleState>.CurState => _curState;
            RefRO<Simulate> IStateAspect<TowerTag, IdleState>. Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<TowerTag, IdleState>.StateRequireEnter => _stateRequireEnter;
        }
    }
}