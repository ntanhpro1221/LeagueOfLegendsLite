using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public static partial class InhibitorStateDead2Idle {
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
                  , transition
                  , select_highlight_healthBar)
                in SystemAPI.Query<
                    StateFilterAspect
                  , ActorSharedStateAspect
                  , TransitionStateAspectRW
                  , Select_Highlight_HealthBarAspect>()) {

                // IDLE STATE
                if (curTick.IsNewerThan(transition.DoneAtTick)) { // It's tick to read idle
                    sharedState.SetIdle();
                    transition.HardCutAnim = true;
                }
                else continue;

                IStateExitFunc<Dead2IdleState>.MarkExitExecuted(filter);

                select_highlight_healthBar.EnableAll();
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
            foreach (var (_, transition, anim) in SystemAPI.Query<
                StateFilterAspect
              , TransitionStateAspectRW
              , RefRO<SharedAnimData>>()) {
                transition.CurAnim = SharedAnimKey.Dead2Idle;

                transition.DoneAtTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick.WithBonusTick(
                    anim.ValueRO.AnimLengthTicks[SharedAnimKey.Dead2Idle]);
            }
        }
    }
}

public static partial class InhibitorStateDead2Idle {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<InhibitorTag, Dead2IdleState> {
            private readonly RefRO<InhibitorTag> _identity;
            private readonly RefRO<Simulate>     _simulate;

            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<Dead2IdleState>         _curStateEnable;

            RefRO<InhibitorTag> IStateAspect<InhibitorTag, Dead2IdleState>.Identity => _identity;
            RefRO<Simulate> IStateAspect<InhibitorTag, Dead2IdleState>.    Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitFunc<Dead2IdleState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<Dead2IdleState> IStateExitFunc<Dead2IdleState>.        CurStateEnable    => _curStateEnable;
        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<InhibitorTag, Dead2IdleState> {
            private readonly RefRO<InhibitorTag> _identity;
            private readonly RefRO<Dead2IdleState>    _curState;
            private readonly RefRO<Simulate>     _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<InhibitorTag> IStateAspect<InhibitorTag, Dead2IdleState>.Identity => _identity;
            RefRO<Dead2IdleState> IStateAspect<InhibitorTag, Dead2IdleState>.   CurState => _curState;
            RefRO<Simulate> IStateAspect<InhibitorTag, Dead2IdleState>.    Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<InhibitorTag, Dead2IdleState>.StateRequireEnter => _stateRequireEnter;
        }
    }
}