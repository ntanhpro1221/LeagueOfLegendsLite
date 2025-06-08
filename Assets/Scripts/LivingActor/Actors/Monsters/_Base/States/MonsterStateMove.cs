using NGDtuanh.Entities.StateMachine;
using Pathfinding;
using Pathfinding.ECS;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

public static partial class MonsterStateMove {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
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

            foreach (var (filter, data, entity)
                in SystemAPI.Query<
                        StateFilterAspect
                      , UpdateAspect>()
                    .WithEntityAccess()) {
                bool haveTargetInRange = data.aimedTarget.HaveTargetInRange(selectLookup, locTransLookup, statsLookup);
                bool attackCDDone      = data.attackData.ValueRO.IsCooldownDone(curTick);

                // DEAD STATE
                if (data.health.IsDead) // RUN OUT OF HEALTH
                    data.sharedState.SetDead();

                // ATTACK STATE
                else if (
                    // Not in leash disabling state
                    !data.IsLeashDisabling
                    // Have target in range
                 && haveTargetInRange
                    // Attack CD done
                 && attackCDDone)
                    data.sharedState.SetAttack();

                // IDLE STATE
                else if (
                    // Stay in leash anchor and not tracing anyone
                    data is {
                        IsLeashDisabling: false
                      , IsLeashing      : false
                    }
                    // have target within range but cooldown not done
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                 || (haveTargetInRange && !attackCDDone))
                    data.sharedState.SetIdle();

                else continue;

                filter.MarkExitExecuted();
                data.StopMove(entity);
            }
        }

        private readonly partial struct UpdateAspect : IAspect {
            public readonly HealthAspectRO         health;
            public readonly AimedTargetAspectRO    aimedTarget;
            public readonly ActorSharedStateAspect sharedState;
            public readonly RefRO<AttackStateData> attackData;
            public readonly RefRO<LocalTransform>  locTrans;
            public readonly FixablePosSetterAspect fixSetter;

            [Optional] private readonly EnabledRefRW<AutoFollowTarget_FollowerEntity> autoFollow;

            [Optional] private readonly EnabledRefRO<MonsterLeashAnchor>    _LeashTrigger;
            [Optional] private readonly EnabledRefRO<MonsterLeashDisabling> _UnleashTrigger;

            public bool IsLeashing       => _LeashTrigger.ValueRO;
            public bool IsLeashDisabling => _UnleashTrigger.ValueRO;

            public void StopMove(in Entity entity) {
                fixSetter.FixAt(locTrans.ValueRO.Position);

                autoFollow.ValueRW = false;
            }
        }
    }

    [UpdateInGroup(typeof(StateEnterSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct Enter : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (
                _
              , anim
              , fixSetter
                ) in SystemAPI
                .Query<
                    StateFilterAspect
                  , SharedAnimAspect
                  , FixablePosSetterAspect>()) {
                anim.SetAnim(SharedAnimKey.Move);
                fixSetter.Release();
            }
        }
    }

    [UpdateInGroup(typeof(StateUpdateSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct Update : ISystem {
        [ReadOnly] private ComponentLookup<Selectable> selectLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            selectLookup = SystemAPI.GetComponentLookup<Selectable>(
                isReadOnly: true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            selectLookup.Update(ref state);

            state.Dependency = new FollowTargetJob {
                selectLookup = selectLookup
            }.ScheduleParallel(state.Dependency);

            state.Dependency = new ReturnToAnchorJob()
                .ScheduleParallel(state.Dependency);

            state.Dependency = new UpdateRotationDataJob()
                .ScheduleParallel(state.Dependency);
        }

        [WithPresent(typeof(AutoFollowTarget_FollowerEntity))]
        [WithPresent(typeof(MonsterLeashDisabling))]
        [BurstCompile]
        private partial struct FollowTargetJob : IJobEntity {
            [ReadOnly] public ComponentLookup<Selectable> selectLookup;

            [BurstCompile]
            public void Execute(
                StateFilterAspect                             _
              , AimedTargetAspectRO                           aimedTarget
              , EnabledRefRW<AutoFollowTarget_FollowerEntity> autoFollow
              , in LocalTransform                             locTrans
              , EnabledRefRO<MonsterLeashDisabling>           leashDisableTrigger) {
                autoFollow.ValueRW = aimedTarget.IsTargetExists(selectLookup);

                if (leashDisableTrigger.ValueRO) autoFollow.ValueRW = false;
            }
        }

        [WithPresent(typeof(MonsterLeashAnchor))]
        [WithAll(typeof(MonsterLeashDisabling))]
        [BurstCompile]
        private partial struct ReturnToAnchorJob : IJobEntity {
            [BurstCompile]
            public void Execute(
                StateFilterAspect      _
              , ref DestinationPoint   desSetter
              , in  MonsterLeashAnchor anchor) {
                desSetter.destination = anchor.anchorPos;
            }
        }

        [BurstCompile]
        private partial struct UpdateRotationDataJob : IJobEntity {
            [BurstCompile]
            public void Execute(
                StateFilterAspect      _
              , in  MovementStatistics moveStats
              , ref RotationData       rotationData) {
                if (math.abs(moveStats.estimatedVelocity.x)
                  + math.abs(moveStats.estimatedVelocity.z)
                  < 4) return;
                rotationData.RotateTo(moveStats.estimatedVelocity.Quantizate3().xz);
            }
        }
    }
}

public static partial class MonsterStateMove {
    public partial struct Exit {
        public struct InheritTag : IStateInheritTag<MonsterTag, MoveState> { }

        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<MonsterTag, MoveState>.RequireInherit<InheritTag> {
            private readonly RefRO<MonsterTag> _identity;
            private readonly RefRO<Simulate>   _simulate;
            private readonly RefRO<InheritTag> _inheritTag;

            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<MoveState>         _curStateEnable;

            RefRO<MonsterTag> IStateAspect<MonsterTag, MoveState>.                 Identity   => _identity;
            RefRO<Simulate> IStateAspect<MonsterTag, MoveState>.                   Simulate   => _simulate;
            RefRO<InheritTag> IStateInheritable<MonsterTag, MoveState, InheritTag>.InheritTag => _inheritTag;

            EnabledRefRW<StateNotExitedYet> IStateExitAspect<MonsterTag, MoveState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<MoveState> IStateExitAspect<MonsterTag, MoveState>.        CurStateEnable    => _curStateEnable;

            public void MarkExitExecuted() => _stateNotExitedYet.ValueRW = _curStateEnable.ValueRW = false;

        }
    }

    public partial struct Enter {
        public struct InheritTag : IStateInheritTag<MonsterTag, MoveState> { }

        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<MonsterTag, MoveState>.RequireInherit<InheritTag> {
            private readonly RefRO<MonsterTag> _identity;
            private readonly RefRO<MoveState>  _curState;
            private readonly RefRO<Simulate>   _simulate;
            private readonly RefRO<InheritTag> _inheritTag;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<MonsterTag> IStateAspect<MonsterTag, MoveState>.                 Identity   => _identity;
            RefRO<MoveState> IStateAspect<MonsterTag, MoveState>.                  CurState   => _curState;
            RefRO<Simulate> IStateAspect<MonsterTag, MoveState>.                   Simulate   => _simulate;
            RefRO<InheritTag> IStateInheritable<MonsterTag, MoveState, InheritTag>.InheritTag => _inheritTag;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<MonsterTag, MoveState>.StateRequireEnter => _stateRequireEnter;
        }
    }

    public partial struct Update {
        public struct InheritTag : IStateInheritTag<MonsterTag, MoveState> { }

        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<MonsterTag, MoveState>.RequireInherit<InheritTag> {
            private readonly RefRO<MonsterTag> _identity;
            private readonly RefRO<MoveState>  _curState;
            private readonly RefRO<Simulate>   _simulate;
            private readonly RefRO<InheritTag> _inheritTag;

            RefRO<MonsterTag> IStateAspect<MonsterTag, MoveState>.                 Identity   => _identity;
            RefRO<MoveState> IStateAspect<MonsterTag, MoveState>.                  CurState   => _curState;
            RefRO<Simulate> IStateAspect<MonsterTag, MoveState>.                   Simulate   => _simulate;
            RefRO<InheritTag> IStateInheritable<MonsterTag, MoveState, InheritTag>.InheritTag => _inheritTag;
        }
    }
}