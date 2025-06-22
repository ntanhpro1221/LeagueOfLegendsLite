using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public static partial class TurretStateIdle {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct Exit : ISystem {
        [ReadOnly] private ComponentLookup<Selectable> selectLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();

            selectLookup = SystemAPI.GetComponentLookup<Selectable>(
                isReadOnly: true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            selectLookup.Update(ref state);

            var curTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            foreach (var (
                    filter
                  , health
                  , aimedTarget
                  , sharedState
                  , attackData)
                in SystemAPI.Query<
                    StateFilterAspect
                  , HealthAspectRO
                  , AimedTargetAspectRO
                  , ActorSharedStateAspect
                  , RefRO<AttackStateData>>()) {

                // DEAD STATE
                if (health.IsDead) // RUN OUT OF HEALTH
                    sharedState.SetDead();

                // ATTACK STATE
                else if (
                    // have target
                    aimedTarget.IsTargetExists(selectLookup)
                    // attack cool down done
                 && attackData.ValueRO.IsCooldownDone(curTick))
                    sharedState.SetAttack();
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

public static partial class TurretStateIdle {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<TurretTag, IdleState> {
            private readonly RefRO<TurretTag> _identity;
            private readonly RefRO<Simulate>  _simulate;

            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<IdleState>         _curStateEnable;

            RefRO<TurretTag> IStateAspect<TurretTag, IdleState>.Identity => _identity;
            RefRO<Simulate> IStateAspect<TurretTag, IdleState>. Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitFunc<IdleState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<IdleState> IStateExitFunc<IdleState>.        CurStateEnable    => _curStateEnable;
        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<TurretTag, IdleState> {
            private readonly RefRO<TurretTag> _identity;
            private readonly RefRO<IdleState> _curState;
            private readonly RefRO<Simulate>  _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<TurretTag> IStateAspect<TurretTag, IdleState>.Identity => _identity;
            RefRO<IdleState> IStateAspect<TurretTag, IdleState>.CurState => _curState;
            RefRO<Simulate> IStateAspect<TurretTag, IdleState>. Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<TurretTag, IdleState>.StateRequireEnter => _stateRequireEnter;
        }
    }
}