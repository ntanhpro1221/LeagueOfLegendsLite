using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

public static partial class ChampionStateMove {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    public partial struct Exit : ISystem {
        private EntityStorageInfoLookup       entityLookup;
        private ComponentLookup<LocalToWorld> l2wLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<EnumIndexData>();

            entityLookup = SystemAPI.GetEntityStorageInfoLookup();
            l2wLookup    = SystemAPI.GetComponentLookup<LocalToWorld>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            entityLookup.Update(ref state);
            l2wLookup.Update(ref state);

            var curTick       = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
            var attackRangeId = SystemAPI.GetSingleton<EnumIndexData>().StatsType[StatsType.AttackRange];

            foreach (var (
                    filter
                  , health
                  , velocity
                  , aimedTarget
                  , sharedState
                  , attackData)
                in SystemAPI.Query<
                    StateFilterAspect
                  , HealthAspectRO
                  , VelocityAspectRO
                  , AimedTargetAspectRO
                  , ActorSharedStateAspect
                  , RefRO<AttackStateData>>()) {
                bool haveTargetInRange  = aimedTarget.HaveTargetInRange(entityLookup, attackRangeId, l2wLookup);
                bool attackCooldownDone = attackData.ValueRO.IsCooldownDone(curTick);

                // DEAD STATE
                if (health.IsDead) // RUN OUT OF HEALTH
                    sharedState.SetDead();

                // ATTACK STATE
                else if (haveTargetInRange && attackCooldownDone) // have target within range and cooldown done
                    sharedState.SetAttack();

                // IDLE STATE
                else if (
                    // NOT HAVE VELOCITY
                    !velocity.IsMoving
                    // have target within range but cooldown not done
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                 || (haveTargetInRange && !attackCooldownDone))
                    sharedState.SetIdle();
                else continue;

                filter.MarkExitExecuted();
            }
        }
    }

    [UpdateInGroup(typeof(StateEnterSystemGroup))]
    public partial struct Enter : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (_, anim) in SystemAPI.Query<
                StateFilterAspect
              , SharedAnimAspect>()) {
                Debug.Log("MOVE");
                anim.SetAnim(SharedAnimKey.Move);
            }
        }
    }

    [UpdateInGroup(typeof(StateUpdateSystemGroup))]
    public partial struct Update : ISystem {
        private EntityStorageInfoLookup       entityLookup;
        private ComponentLookup<LocalToWorld> l2wLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            entityLookup = SystemAPI.GetEntityStorageInfoLookup();
            l2wLookup    = SystemAPI.GetComponentLookup<LocalToWorld>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            entityLookup.Update(ref state);
            l2wLookup.Update(ref state);

            foreach (var (
                _
              , moveData
              , aimedTarget) in SystemAPI.Query<
                StateFilterAspect
              , RefRW<MoveData>
              , AimedTargetAspectRO>()) {
                // MOVE TO AIMED TARGET
                if (aimedTarget.IsTargetExists(entityLookup))
                    moveData.ValueRW.targetLocalPos = l2wLookup[aimedTarget.Target].Position.Quantizate3();
            }
        }
    }
}

public static partial class ChampionStateMove {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<ChampionTag, MoveState> {
            private readonly RefRO<ChampionTag> _identity;
            private readonly RefRO<Simulate>    _simulate;

            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<MoveState>         _curStateEnable;

            RefRO<ChampionTag> IStateAspect<ChampionTag, MoveState>.Identity => _identity;
            RefRO<Simulate> IStateAspect<ChampionTag, MoveState>.   Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitAspect<ChampionTag, MoveState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<MoveState> IStateExitAspect<ChampionTag, MoveState>.        CurStateEnable    => _curStateEnable;

            public void MarkExitExecuted() => _stateNotExitedYet.ValueRW = _curStateEnable.ValueRW = false;

        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<ChampionTag, MoveState> {
            private readonly RefRO<ChampionTag> _identity;
            private readonly RefRO<MoveState>   _curState;
            private readonly RefRO<Simulate>    _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<ChampionTag> IStateAspect<ChampionTag, MoveState>.Identity => _identity;
            RefRO<MoveState> IStateAspect<ChampionTag, MoveState>.  CurState => _curState;
            RefRO<Simulate> IStateAspect<ChampionTag, MoveState>.   Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<ChampionTag, MoveState>.StateRequireEnter => _stateRequireEnter;
        }
    }

    public partial struct Update {
        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<ChampionTag, MoveState> {
            private readonly RefRO<ChampionTag> _identity;
            private readonly RefRO<MoveState>   _curState;
            private readonly RefRO<Simulate>    _simulate;

            RefRO<ChampionTag> IStateAspect<ChampionTag, MoveState>.Identity => _identity;
            RefRO<MoveState> IStateAspect<ChampionTag, MoveState>.  CurState => _curState;
            RefRO<Simulate> IStateAspect<ChampionTag, MoveState>.   Simulate => _simulate;
        }
    }
}