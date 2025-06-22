using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

public static partial class InhibitorStateIdle2Dead {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    public partial struct Exit : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            state.CompleteDependency();

            var curTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            foreach (var (
                    filter
                  , sharedState
                  , transition)
                in SystemAPI.Query<
                    StateFilterAspect
                  , ActorSharedStateAspect
                  , TransitionStateAspectRW>()) {

                // DEAD STATE
                if (curTick.IsNewerThan(transition.DoneAtTick)) { // It's tick to read Dead
                    sharedState.SetDead();
                    transition.HardCutAnim = false;
                }
                else continue;

                IStateExitFunc<Idle2DeadState>.MarkExitExecuted(filter);
            }
        }
    }

    [UpdateInGroup(typeof(StateEnterSystemGroup))]
    public partial struct Enter : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (
                _
              , select_highlight_healthBar
              , transition
              , anim) in SystemAPI.Query<
                StateFilterAspect
              , Select_Highlight_HealthBarAspect
              , TransitionStateAspectRW
              , RefRO<SharedAnimData>>()) {
                transition.CurAnim = SharedAnimKey.Idle2Dead;

                transition.DoneAtTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick.WithBonusTick(
                    anim.ValueRO.AnimLengthTicks[SharedAnimKey.Idle2Dead]);

                select_highlight_healthBar.DisableAll();
            }
        }
    }
}

public static partial class InhibitorStateIdle2Dead {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<InhibitorTag, Idle2DeadState> {
            private readonly RefRO<InhibitorTag> _identity;
            private readonly RefRO<Simulate>     _simulate;

            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<Idle2DeadState>         _curStateEnable;

            RefRO<InhibitorTag> IStateAspect<InhibitorTag, Idle2DeadState>.Identity => _identity;
            RefRO<Simulate> IStateAspect<InhibitorTag, Idle2DeadState>.    Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitFunc<Idle2DeadState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<Idle2DeadState> IStateExitFunc<Idle2DeadState>.        CurStateEnable    => _curStateEnable;
        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<InhibitorTag, Idle2DeadState> {
            private readonly RefRO<InhibitorTag> _identity;
            private readonly RefRO<Idle2DeadState>    _curState;
            private readonly RefRO<Simulate>     _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<InhibitorTag> IStateAspect<InhibitorTag, Idle2DeadState>.Identity => _identity;
            RefRO<Idle2DeadState> IStateAspect<InhibitorTag, Idle2DeadState>.   CurState => _curState;
            RefRO<Simulate> IStateAspect<InhibitorTag, Idle2DeadState>.    Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<InhibitorTag, Idle2DeadState>.StateRequireEnter => _stateRequireEnter;
        }
    }
}