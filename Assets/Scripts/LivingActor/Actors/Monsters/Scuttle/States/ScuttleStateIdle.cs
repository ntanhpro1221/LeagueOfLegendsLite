using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Entities;

public static partial class ScuttleStateIdle {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct Exit : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (
                filter
              , sharedState
              , health
              , cc
                ) in SystemAPI.Query<
                StateFilterAspect
              , ActorSharedStateAspect
              , HealthAspectRO
              , CCAspectRO>()) {
                // DEAD STATE
                if (health.IsDead) // RUN OUT OF HEALTH
                    sharedState.SetDead();

                // MOVE STATE
                else if (
                    // Not have disabling move CC.
                    cc.Disable.Move == 0)
                    sharedState.SetMove();

                else continue;

                IStateExitFunc<IdleState>.MarkExitExecuted(filter);
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

public static partial class ScuttleStateIdle {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<ScuttleTag, IdleState> {
            private readonly RefRO<ScuttleTag> _identity;
            private readonly RefRO<Simulate>   _simulate;

            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<IdleState>         _curStateEnable;

            RefRO<ScuttleTag> IStateAspect<ScuttleTag, IdleState>.Identity => _identity;
            RefRO<Simulate> IStateAspect<ScuttleTag, IdleState>.  Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitFunc<IdleState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<IdleState> IStateExitFunc<IdleState>.        CurStateEnable    => _curStateEnable;
        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<ScuttleTag, IdleState> {
            private readonly RefRO<ScuttleTag> _identity;
            private readonly RefRO<IdleState>  _curState;
            private readonly RefRO<Simulate>   _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<ScuttleTag> IStateAspect<ScuttleTag, IdleState>.Identity => _identity;
            RefRO<IdleState> IStateAspect<ScuttleTag, IdleState>. CurState => _curState;
            RefRO<Simulate> IStateAspect<ScuttleTag, IdleState>.  Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<ScuttleTag, IdleState>.StateRequireEnter => _stateRequireEnter;
        }
    }
}