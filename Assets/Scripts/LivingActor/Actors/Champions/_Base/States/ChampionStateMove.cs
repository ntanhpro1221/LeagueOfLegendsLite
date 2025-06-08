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
        [ReadOnly] private ComponentLookup<Selectable>     selectLookup;
        [ReadOnly] private ComponentLookup<LocalTransform> locTransLookup;
        [ReadOnly] private BufferLookup<StatsBuffer>       statsLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();

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

            foreach (var (filter, data)
                in SystemAPI.Query<
                    StateFilterAspect
                  , UpdateAspect>()) {
                bool haveTargetInRange  = data.aimedTarget.HaveTargetInRange(selectLookup, locTransLookup, statsLookup);
                bool attackCooldownDone = data.attackData.ValueRO.IsCooldownDone(curTick);

                // DEAD STATE
                if (data.health.IsDead) // RUN OUT OF HEALTH
                    data.sharedState.SetDead();

                // ITEM ANALYZING STATE
                else if (data.itemRequest.ValueRO.haveRequest)
                    data.sharedState.SetItemActiveAnalyzing();

                // ATTACK STATE
                else if (haveTargetInRange && attackCooldownDone) // have target within range and cooldown done
                    data.sharedState.SetAttack();

                // IDLE STATE
                else if (
                    // Have cancel request
                    data.HaveCancelMoveRequest
                    // Done move and not have move request from player
                 || (data.moveRequester.IsMoveDone && !data.input.MoveEvent_WithData)
                    // have target within range and so close to target
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                 || (haveTargetInRange && data.aimedTarget.SoCloseToTarget(selectLookup, locTransLookup, statsLookup)))
                    data.sharedState.SetIdle();

                else continue;

                filter.MarkExitExecuted();
                data.StopMove();
            }
        }

        private readonly partial struct UpdateAspect : IAspect {
            public readonly HealthAspectRO                       health;
            public readonly AimedTargetAspectRO                  aimedTarget;
            public readonly ActorSharedStateAspect               sharedState;
            public readonly RefRO<AttackStateData>               attackData;
            public readonly MoveRequesterAspect                  moveRequester;
            public readonly PlayerInputAspectRO                  input;
            public readonly RefRO<ItemActiveNewStateRequestData> itemRequest;

            private readonly RefRO<LocalTransform> localTrans;

            [Optional] private readonly EnabledRefRW<AutoFollowTarget> autoFollowTarget;

            public bool HaveCancelMoveRequest => input.Input.GetEvent_Only(PlayerTrigger.Other.CancelMove);

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
                  , moveRequester
                  , aimedTarget
                  , autoFollowTarget
                  , locTrans
                  , input)
                in SystemAPI
                    .Query<
                        StateFilterAspect
                      , MoveRequesterAspect
                      , AimedTargetAspectRO
                      , EnabledRefRW<AutoFollowTarget>
                      , RefRO<LocalTransform>
                      , PlayerInputAspectRO>()
                    .WithPresent<AutoFollowTarget>()) {

                // Try move to aimed target
                autoFollowTarget.ValueRW = aimedTarget.IsTargetExists(selectLookup);

                // If not aiming to any target => move to input of user
                if (!autoFollowTarget.ValueRO
                 && input.MoveEvent_WithData)
                    moveRequester.MoveSmartTo(input.Input.moveLocTarget, locTrans.ValueRO);
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