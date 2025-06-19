using NGDtuanh.Entities.StateMachine;
using Pathfinding;
using Pathfinding.ECS;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

public static partial class MinionStateMove {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct Exit : ISystem {
        [ReadOnly] private ComponentLookup<Selectable>     selectLookup;
        [ReadOnly] private ComponentLookup<LocalTransform> locTransLookup;
        [ReadOnly] private ComponentLookup<StatsData>      statsLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();

            selectLookup = SystemAPI.GetComponentLookup<Selectable>(
                isReadOnly: true);
            locTransLookup = SystemAPI.GetComponentLookup<LocalTransform>(
                isReadOnly: true);
            statsLookup = SystemAPI.GetComponentLookup<StatsData>(
                isReadOnly: true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            selectLookup.Update(ref state);
            locTransLookup.Update(ref state);
            statsLookup.Update(ref state);

            var curTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            foreach (var (
                filter
              , common
              , attackData
              , data
              , entity
                ) in SystemAPI
                .Query<
                    StateFilterAspect
                  , CommonExitStateAspect
                  , RefRO<AttackStateData>
                  , UpdateAspect
                >().WithEntityAccess()) {
                bool haveTargetInRange  = common.Target.HaveTargetInRange(selectLookup, locTransLookup, statsLookup);
                bool attackCooldownDone = attackData.ValueRO.IsCooldownDone(curTick);

                // DEAD STATE
                if (common.Health.IsDead) // Run out of health.
                    common.State.SetDead();

                // ATTACK STATE
                else if (
                    // Not have disabling move CC.
                    common.CC.Disable.Attack == 0
                    // Have target within range.
                 && haveTargetInRange 
                    // Cooldown done.
                 && attackCooldownDone) // have target within range and cooldown done
                    common.State.SetAttack();

                // IDLE STATE
                else if (
                    // Have disabling move CC.
                    common.CC.Disable.Move != 0
                    // Don't have path left
                 || data.PathBuffer.IsEmpty
                    // have target within range but cooldown not done
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                 || (haveTargetInRange && !attackCooldownDone))
                    common.State.SetIdle();

                else continue;

                filter.MarkExitExecuted();
                data.StopMove(entity);
            }
        }

        private readonly partial struct UpdateAspect : IAspect {
            private readonly RefRO<LocalTransform>  _LocTrans;
            private readonly FixablePosSetterAspect _FixSetter;

            [Optional] private readonly EnabledRefRW<AutoFollowTarget_FollowerEntity> _AutoFollow;

            [ReadOnly] public readonly DynamicBuffer<MinionFixedPathBuffer> PathBuffer;

            public void StopMove(in Entity entity) {
                _FixSetter.FixAt(_LocTrans.ValueRO.Position);

                _AutoFollow.ValueRW = false;
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
              , fixSetter) in SystemAPI.Query<
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

            state.Dependency = new UpdateRotationDataJob()
                .ScheduleParallel(state.Dependency);
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
              , ref MinionAggroAnchor                     aggroAnchor
              , EnabledRefRW<MinionAggroAnchor>           anchorEnable
              , EnabledRefRO<MinionAggroDisabling>        aggroDisable) {
                // Already have target
                if (GameHelpers.IsTargetExists(aimedTarget.target, selectLookup)) return;

                // Otherwise so seek for new target in detected collections
                anchorEnable.ValueRW         = false;
                aimedTarget.targetIsChampion = false;

                if (!detectedMinion.IsEmpty) aimedTarget.target     = detectedMinion.FrontRO().entity;
                else if (!detectedTower.IsEmpty) aimedTarget.target = detectedTower.FrontRO().entity;
                else if (!detectedChampion.IsEmpty
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
                StateFilterAspect                             _
              , AimedTargetAspectRO                           aimedTarget
              , EnabledRefRW<AutoFollowTarget_FollowerEntity> autoFollow
              , in LocalTransform                             locTrans) {
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