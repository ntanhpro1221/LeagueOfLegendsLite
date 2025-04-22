using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

public static partial class ChampionStateMove {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    public partial struct Exit : ISystem {
        [ReadOnly] private EntityStorageInfoLookup         entityLookup;
        [ReadOnly] private ComponentLookup<Selectable>     selectLookup;
        [ReadOnly] private ComponentLookup<LocalTransform> locTransLookup;
        [ReadOnly] private BufferLookup<StatsBuffer>       statsLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EnumIndexData>();

            entityLookup = SystemAPI.GetEntityStorageInfoLookup();
            selectLookup = SystemAPI.GetComponentLookup<Selectable>(
                isReadOnly: true);
            locTransLookup = SystemAPI.GetComponentLookup<LocalTransform>(
                isReadOnly: true);
            statsLookup = SystemAPI.GetBufferLookup<StatsBuffer>(
                isReadOnly: true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            entityLookup.Update(ref state);
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
                bool haveTargetInRange  = data.aimedTarget.HaveTargetInRange(entityLookup, selectLookup, attackRangeId, unitRadiusId, locTransLookup, statsLookup);
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
        private            EntityStorageInfoLookup         entityLookup;
        [ReadOnly] private ComponentLookup<Selectable>     selectLookup;
        private            ComponentLookup<LocalTransform> locTransLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
            entityLookup = SystemAPI.GetEntityStorageInfoLookup();
            selectLookup = SystemAPI.GetComponentLookup<Selectable>(
                isReadOnly: true);
            locTransLookup = SystemAPI.GetComponentLookup<LocalTransform>(
                isReadOnly: true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            entityLookup.Update(ref state);
            selectLookup.Update(ref state);
            locTransLookup.Update(ref state);

            foreach (var (
                    _
                  , moveRequester
                  , aimedTarget
                  , input
                  , autoFollowTarget)
                in SystemAPI
                    .Query<
                        StateFilterAspect
                      , MoveRequesterAspect
                      , AimedTargetAspectRO
                      , RefRO<PlayerInputData>
                      , EnabledRefRW<AutoFollowTarget>>()
                    .WithPresent<AutoFollowTarget>()) {

                // Try move to aimed target
                autoFollowTarget.ValueRW = aimedTarget.IsTargetExists(entityLookup, selectLookup);

                // If not aiming to any target => move to input of user
                if (!autoFollowTarget.ValueRO
                 && moveRequester.NeedRecalculatePath(input.ValueRO.moveLocTarget))
                    moveRequester.MoveSmartAttackTo(input.ValueRO.moveLocTarget);
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