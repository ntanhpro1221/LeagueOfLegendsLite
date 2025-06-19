using NGDtuanh.Entities.StateMachine;
using Pathfinding.ECS;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public static partial class ScuttleStateMove {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct Exit : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (
                filter
              , cc
              , data
              , entity
                ) in SystemAPI
                .Query<
                    StateFilterAspect
                  , CCAspectRO
                  , UpdateAspect
                >().WithEntityAccess()) {

                // DEAD STATE
                if (data.health.IsDead) // RUN OUT OF HEALTH
                    data.sharedState.SetDead();

                // IDLE STATE
                else if (
                    // Have disabling move CC.
                    cc.Disable.Move != 0)
                    data.sharedState.SetIdle();

                else continue;

                filter.MarkExitExecuted();
                data.StopMove(entity);
            }
        }

        private readonly partial struct UpdateAspect : IAspect {
            public readonly HealthAspectRO          health;
            public readonly ActorSharedStateAspect  sharedState;
            public readonly RefRW<DestinationPoint> desSetter;

            public void StopMove(in Entity entity) {
                desSetter.ValueRW.destination = mathHelpers.PositiveInfinity_Float3;
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
              , anim) in SystemAPI.Query<
                StateFilterAspect
              , SharedAnimAspect>()) {
                anim.SetAnim(SharedAnimKey.Move);
            }
        }
    }

    [UpdateInGroup(typeof(StateUpdateSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct Update : ISystem {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            using var queryBuilder = new EntityQueryBuilder(Allocator.Temp);

            FollowerEntityFixedPathJob.TakeMyQuery(queryBuilder.WithAll<
                Simulate
              , ScuttleTag
              , MoveState>());

            query = queryBuilder.Build(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            state.Dependency = new FollowerEntityFixedPathJob()
                .ScheduleParallel(query, state.Dependency);

            state.Dependency = new UpdateRotationDataJob()
                .ScheduleParallel(state.Dependency);
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

public static partial class ScuttleStateMove {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<ScuttleTag, MoveState> {
            private readonly RefRO<ScuttleTag> _identity;
            private readonly RefRO<Simulate>   _simulate;

            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<MoveState>         _curStateEnable;

            RefRO<ScuttleTag> IStateAspect<ScuttleTag, MoveState>.Identity => _identity;
            RefRO<Simulate> IStateAspect<ScuttleTag, MoveState>.  Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitAspect<ScuttleTag, MoveState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<MoveState> IStateExitAspect<ScuttleTag, MoveState>.        CurStateEnable    => _curStateEnable;

            public void MarkExitExecuted() => _stateNotExitedYet.ValueRW = _curStateEnable.ValueRW = false;

        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<ScuttleTag, MoveState> {
            private readonly RefRO<ScuttleTag> _identity;
            private readonly RefRO<MoveState>  _curState;
            private readonly RefRO<Simulate>   _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<ScuttleTag> IStateAspect<ScuttleTag, MoveState>.Identity => _identity;
            RefRO<MoveState> IStateAspect<ScuttleTag, MoveState>. CurState => _curState;
            RefRO<Simulate> IStateAspect<ScuttleTag, MoveState>.  Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<ScuttleTag, MoveState>.StateRequireEnter => _stateRequireEnter;
        }
    }

    public partial struct Update {
        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<ScuttleTag, MoveState> {
            private readonly RefRO<ScuttleTag> _identity;
            private readonly RefRO<MoveState>  _curState;
            private readonly RefRO<Simulate>   _simulate;

            RefRO<ScuttleTag> IStateAspect<ScuttleTag, MoveState>.Identity => _identity;
            RefRO<MoveState> IStateAspect<ScuttleTag, MoveState>. CurState => _curState;
            RefRO<Simulate> IStateAspect<ScuttleTag, MoveState>.  Simulate => _simulate;
        }
    }
}