using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

public static partial class ChampionStateIdle {
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

                // DEAD STATE
                if (health.IsDead) // RUN OUT OF HEALTH
                    sharedState.SetDead();

                // MOVE STATE
                else if (
                    // Have velocity
                    velocity.IsMoving
                    // Need move to target
                 || aimedTarget.NeedMoveToTarget(entityLookup, attackRangeId, l2wLookup)) // HAVE VELOCITY
                    sharedState.SetMove();

                // ATTACK STATE
                else if (
                    // have target
                    aimedTarget.IsTargetExists(entityLookup)
                    // attack cool down done
                 && attackData.ValueRO.IsCooldownDone(curTick))
                    sharedState.SetAttack();
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
              , SharedAnimAspect>())
                anim.SetAnim(SharedAnimKey.Idle);
        }
    }
}

public static partial class ChampionStateIdle {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<ChampionTag, IdleState> {
            private readonly RefRO<ChampionTag> _identity;
            private readonly RefRO<Simulate>    _simulate;

            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<IdleState>         _curStateEnable;

            RefRO<ChampionTag> IStateAspect<ChampionTag, IdleState>.Identity => _identity;
            RefRO<Simulate> IStateAspect<ChampionTag, IdleState>.   Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitAspect<ChampionTag, IdleState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<IdleState> IStateExitAspect<ChampionTag, IdleState>.        CurStateEnable    => _curStateEnable;

            public void MarkExitExecuted() => _stateNotExitedYet.ValueRW = _curStateEnable.ValueRW = false;
        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<ChampionTag, IdleState> {
            private readonly RefRO<ChampionTag> _identity;
            private readonly RefRO<IdleState>   _curState;
            private readonly RefRO<Simulate>    _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<ChampionTag> IStateAspect<ChampionTag, IdleState>.Identity => _identity;
            RefRO<IdleState> IStateAspect<ChampionTag, IdleState>.  CurState => _curState;
            RefRO<Simulate> IStateAspect<ChampionTag, IdleState>.   Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<ChampionTag, IdleState>.StateRequireEnter => _stateRequireEnter;
        }
    }
}