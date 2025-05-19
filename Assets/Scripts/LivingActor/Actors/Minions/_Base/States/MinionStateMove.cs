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

public static partial class MinionStateMove {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
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

            foreach (var (filter, data, entity)
                in SystemAPI.Query<
                    StateFilterAspect
                  , UpdateAspect>()
                    .WithEntityAccess()) {
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
                    // Don't have path left
                    data.PathBuffer.Empty()
                    // have target within range but cooldown not done
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                 || (haveTargetInRange && !attackCooldownDone))
                    data.sharedState.SetIdle();

                else continue;

                filter.MarkExitExecuted();
                data.StopMove(entity);
            }
        }

        private readonly partial struct UpdateAspect : IAspect {
            public readonly HealthAspectRO          health;
            public readonly AimedTargetAspectRO     aimedTarget;
            public readonly ActorSharedStateAspect  sharedState;
            public readonly RefRO<AttackStateData>  attackData;
            public readonly RefRW<DestinationPoint> desSetter;
            public readonly RefRW<MovementSettings> moveSetting;

            [Optional] private readonly EnabledRefRW<AutoFollowTarget_FollowerEntity> autoFollow;

            [ReadOnly] public readonly DynamicBuffer<MinionFixedPathBuffer> PathBuffer;

            public void StopMove(in Entity entity) {
                // FollowerEntity.ClearPath(entity);

                desSetter.ValueRW.destination = new float3(
                    float.PositiveInfinity
                  , float.PositiveInfinity
                  , float.PositiveInfinity);

                moveSetting.ValueRW.isStopped = true;
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
                , moveSetting) in SystemAPI.Query<
                StateFilterAspect
              , SharedAnimAspect
            , RefRW<MovementSettings>>()) {
                anim.SetAnim(SharedAnimKey.Move);
                moveSetting.ValueRW.isStopped = false;
            }
        }
    }

    [UpdateInGroup(typeof(StateUpdateSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct Update : ISystem {
        [ReadOnly] private ComponentLookup<Selectable> selectLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<MinionCommonBehaviourConfigData>();
            state.RequireForUpdate<NetworkTime>();
            selectLookup = SystemAPI.GetComponentLookup<Selectable>(
                isReadOnly: true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            selectLookup.Update(ref state);

            state.Dependency = new SeekTargetJob {
                selectLookup = selectLookup
            }.ScheduleParallel(state.Dependency);

            state.Dependency = new FollowTargetJob {
                selectLookup = selectLookup
            }.ScheduleParallel(state.Dependency);

            state.Dependency = new FollowFixedPathJob {
                reachPathDisToleranceSqr = SystemAPI.GetSingleton<MinionCommonBehaviourConfigData>().reachPathDisToleranceSqr
            }.ScheduleParallel(state.Dependency);
        }

        [WithPresent(
            typeof(MinionAggroAnchor)
          , typeof(MinionAggroDisabling))]
        [BurstCompile]
        private partial struct SeekTargetJob : IJobEntity {
            [ReadOnly] public ComponentLookup<Selectable> selectLookup;

            [BurstCompile]
            public void Execute(
                StateFilterAspect                         _
              , ref AimedTargetData                       aimedTarget
              , in  DynamicBuffer<DetectedMinionBuffer>   detectedMinion
              , in  DynamicBuffer<DetectedTowerBuffer>    detectedTower
              , in  DynamicBuffer<DetectedChampionBuffer> detectedChampion
              , in  LocalTransform                        locTrans
              , ref MinionAggroAnchor                           aggroAnchor
              , EnabledRefRW<MinionAggroAnchor>                 anchorEnable
              , EnabledRefRO<MinionAggroDisabling>              aggroDisable) {
                // Already have target
                if (GameHelpers.IsTargetExists(aimedTarget.target, selectLookup)) return;

                // Otherwise so seek for new target in detected collections
                anchorEnable.ValueRW         = false;
                aimedTarget.targetIsChampion = false;

                if (!detectedMinion.Empty()) aimedTarget.target     = detectedMinion.FrontRO().entity;
                else if (!detectedTower.Empty()) aimedTarget.target = detectedTower.FrontRO().entity;
                else if (!detectedChampion.Empty()
                 && !aggroDisable.ValueRO)
                    MinionControlSystem.AimToChamp(
                        detectedChampion.FrontRO().entity
                      , locTrans, ref aimedTarget
                      , ref aggroAnchor
                      , anchorEnable);
            }
        }

        [WithPresent(typeof(AutoFollowTarget_FollowerEntity))]
        [BurstCompile]
        private partial struct FollowTargetJob : IJobEntity {
            [ReadOnly] public ComponentLookup<Selectable> selectLookup;

            [BurstCompile]
            public void Execute(
                StateFilterAspect              _
              , AimedTargetAspectRO            aimedTarget
              , EnabledRefRW<AutoFollowTarget_FollowerEntity> autoFollow
              , in LocalTransform              locTrans) {
                autoFollow.ValueRW = aimedTarget.IsTargetExists(selectLookup);
            }
        }

        [WithNone(typeof(AutoFollowTarget_FollowerEntity))]
        [BurstCompile]
        private partial struct FollowFixedPathJob : IJobEntity {
            public float_Q3 reachPathDisToleranceSqr;

            [BurstCompile]
            public void Execute(
                StateFilterAspect                        _
              , ref DynamicBuffer<MinionFixedPathBuffer> pathBuffer
              , ref DestinationPoint                     desSetter
              , in  LocalTransform                       locTrans) {
                if (pathBuffer.IsEmpty) return;

                if (reachPathDisToleranceSqr
                  > GameHelpers.DistanceXZ_Sqr(locTrans.Position, pathBuffer.FrontRO().pos))
                    pathBuffer.PopFront();
                desSetter.destination = pathBuffer[0].pos;
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