using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Entities;

public static partial class InhibitorStateIdle {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    public partial struct Exit : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (
                    filter
                  , health
                  , transition
                  , sharedState)
                in SystemAPI.Query<
                    StateFilterAspect
                  , HealthAspectRO
                  , TransitionStateAspectRW
                  , ActorSharedStateAspect>()) {

                // IDLE_2_DEAD STATE
                if (health.IsDead) { // RUN OUT OF HEALTH
                    sharedState.SetIdle2Dead();
                    transition.HardCutAnim = true;
                } else continue;

                IStateExitFunc<IdleState>.MarkExitExecuted(filter);
            }
        }
    }
}

public static partial class InhibitorStateIdle {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<InhibitorTag, IdleState> {
            private readonly RefRO<InhibitorTag> _identity;
            private readonly RefRO<Simulate>     _simulate;

            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<IdleState>         _curStateEnable;

            RefRO<InhibitorTag> IStateAspect<InhibitorTag, IdleState>.Identity => _identity;
            RefRO<Simulate> IStateAspect<InhibitorTag, IdleState>.    Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitFunc<IdleState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<IdleState> IStateExitFunc<IdleState>.        CurStateEnable    => _curStateEnable;
        }
    }
}