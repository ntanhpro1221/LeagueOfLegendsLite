using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

public static partial class MinionStateMove {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    public partial struct Exit : ISystem {
        [ReadOnly] private ComponentLookup<Selectable>     selectLookup;
        [ReadOnly] private ComponentLookup<LocalTransform> locTransLookup;
        [ReadOnly] private BufferLookup<StatsBuffer>       statsLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<EnumIndexData>();

            selectLookup = SystemAPI.GetComponentLookup<Selectable>(
                isReadOnly: true);
            locTransLookup = SystemAPI.GetComponentLookup<LocalTransform>(
                isReadOnly: true);
            statsLookup = SystemAPI.GetBufferLookup<StatsBuffer>(
                isReadOnly: true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            selectLookup.Update(ref state);
            locTransLookup.Update(ref state);
            statsLookup.Update(ref state);

            var curTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            ref var statsId       = ref SystemAPI.GetSingleton<EnumIndexData>().StatsType;
            var     attackRangeId = statsId[StatsType.AttackRange];
            var     unitRadiusId  = statsId[StatsType.UnitRadius];

            foreach (var (filter, data)
                in SystemAPI.Query<
                    StateFilterAspect
                  , UpdateAspect>()) {
                bool haveTargetInRange  = data.aimedTarget.HaveTargetInRange(selectLookup, attackRangeId, unitRadiusId, locTransLookup, statsLookup);
                bool attackCooldownDone = data.attackData.ValueRO.IsCooldownDone(curTick);

                // DEAD STATE
                if (data.health.IsDead) // RUN OUT OF HEALTH
                    data.sharedState.SetDead();

                // ATTACK STATE
                else if (haveTargetInRange && attackCooldownDone) // have target within range and cooldown done
                    data.sharedState.SetAttack();

                // IDLE STATE
                else if (
                    // Done move
                    data.moveRequester.IsMoveDone
                    // have target within range but cooldown not done
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                 || (haveTargetInRange && !attackCooldownDone))
                    data.sharedState.SetIdle();
                else continue;

                filter.MarkExitExecuted();
                data.StopMove();
            }
        }

        private readonly partial struct UpdateAspect : IAspect {
            public readonly HealthAspectRO         health;
            public readonly AimedTargetAspectRO    aimedTarget;
            public readonly ActorSharedStateAspect sharedState;
            public readonly RefRO<AttackStateData> attackData;
            public readonly MoveRequesterAspect    moveRequester;

            private readonly RefRO<LocalTransform> localTrans;

            [Optional] private readonly EnabledRefRW<AutoFollowTarget> autoFollowTarget;


            public void StopMove() {
                moveRequester.SyncFromLocTrans(localTrans.ValueRO);

                autoFollowTarget.ValueRW = false;
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
                anim.SetAnim(SharedAnimKey.Move);
            }
        }
    }

    [UpdateInGroup(typeof(StateUpdateSystemGroup))]
    public partial struct Update : ISystem {
        [ReadOnly] private ComponentLookup<Selectable>     selectLookup;
        [ReadOnly] private ComponentLookup<LocalTransform> locTransLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
            selectLookup = SystemAPI.GetComponentLookup<Selectable>(
                isReadOnly: true);
            locTransLookup = SystemAPI.GetComponentLookup<LocalTransform>(
                isReadOnly: true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            selectLookup.Update(ref state);
            locTransLookup.Update(ref state);

            foreach (var (
                    _
                  , aimedTarget
                  , autoFollowTarget)
                in SystemAPI
                    .Query<
                        StateFilterAspect
                      , AimedTargetAspectRO
                      , EnabledRefRW<AutoFollowTarget>>()
                    .WithPresent<AutoFollowTarget>()) {
                // Try move to aimed target
                autoFollowTarget.ValueRW = aimedTarget.IsTargetExists(selectLookup);
            }
        }
    }
}

public static partial class MinionStateMove {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<MinionTag, MoveState> {
            private readonly RefRO<MinionTag> _identity;
            private readonly RefRO<Simulate>  _simulate;

            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<MoveState>         _curStateEnable;

            RefRO<MinionTag> IStateAspect<MinionTag, MoveState>.Identity => _identity;
            RefRO<Simulate> IStateAspect<MinionTag, MoveState>. Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitAspect<MinionTag, MoveState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<MoveState> IStateExitAspect<MinionTag, MoveState>.        CurStateEnable    => _curStateEnable;

            public void MarkExitExecuted() => _stateNotExitedYet.ValueRW = _curStateEnable.ValueRW = false;

        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<MinionTag, MoveState> {
            private readonly RefRO<MinionTag> _identity;
            private readonly RefRO<MoveState> _curState;
            private readonly RefRO<Simulate>  _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<MinionTag> IStateAspect<MinionTag, MoveState>.Identity => _identity;
            RefRO<MoveState> IStateAspect<MinionTag, MoveState>.CurState => _curState;
            RefRO<Simulate> IStateAspect<MinionTag, MoveState>. Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<MinionTag, MoveState>.StateRequireEnter => _stateRequireEnter;
        }
    }

    public partial struct Update {
        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<MinionTag, MoveState> {
            private readonly RefRO<MinionTag> _identity;
            private readonly RefRO<MoveState> _curState;
            private readonly RefRO<Simulate>  _simulate;

            RefRO<MinionTag> IStateAspect<MinionTag, MoveState>.Identity => _identity;
            RefRO<MoveState> IStateAspect<MinionTag, MoveState>.CurState => _curState;
            RefRO<Simulate> IStateAspect<MinionTag, MoveState>. Simulate => _simulate;
        }
    }
}