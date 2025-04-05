using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public static partial class ChampionStateMove {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    public partial struct Exit : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var data in SystemAPI.Query<UpdateAspect>()) {
                // DEAD STATE
                if (data.CurHealth <= 0) // RUN OUT OF HEALTH
                    data.DeadStateEnable.ValueRW = true;
                // IDLE STATE
                else if (data.SumVelocityXZ <= float_Q3.Epsilon) // NOT HAVE VELOCITY
                    data.IdleStateEnable.ValueRW = true;
                else continue;

                data.MarkExitExecuted();
            }
        }

        private readonly partial struct UpdateAspect {
            private readonly RefRO<HealthData>      _HealthData;
            private readonly RefRO<PhysicsVelocity> _PhysicsVelocity;

            [Optional] public readonly EnabledRefRW<DeadState> DeadStateEnable;
            [Optional] public readonly EnabledRefRW<IdleState> IdleStateEnable;

            public float_Q3 CurHealth => _HealthData.ValueRO.value;

            public float SumVelocityXZ =>
                math.abs(_PhysicsVelocity.ValueRO.Linear.x)
              + math.abs(_PhysicsVelocity.ValueRO.Linear.z);
        }

        private readonly partial struct UpdateAspect : IAspect, IStateExitAspect<ChampionTag, MoveState> {
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

    [UpdateInGroup(typeof(StateEnterSystemGroup))]
    public partial struct Enter : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var data in SystemAPI.Query<UpdateAspect>()) {
                data.CurAnim = SharedAnimKey.Move;
            }
        }

        private readonly partial struct UpdateAspect {
            private readonly RefRW<AnimData> _AnimData;

            public ref SharedAnimKey CurAnim => ref _AnimData.ValueRW.curAnim;
        }

        private readonly partial struct UpdateAspect : IAspect, IStateEnterAspect<ChampionTag, MoveState> {
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

    // [UpdateInGroup(typeof(StateUpdateSystemGroup))]
    // public partial struct Update : ISystem {
    //     [BurstCompile]
    //     public void OnUpdate(ref SystemState state) {
    //         foreach (var (
    //                 )
    //             in SystemAPI
    //                 .Query<
    //                     >()
    //                 .WithAll<
    //                     YasuoTag
    //                   , MoveState
    //                   , Simulate>()) {
    //             
    //         }
    //     }
    // }
    //
    // [UpdateInGroup(typeof(StateFixedUpdateSystemGroup))]
    // public partial struct FixedUpdate : ISystem {
    //     [BurstCompile]
    //     public void OnUpdate(ref SystemState state) {
    //         foreach (var (
    //                 )
    //             in SystemAPI
    //                 .Query<
    //                     >()
    //                 .WithAll<
    //                     YasuoTag
    //                   , MoveState
    //                   , Simulate>()) {
    //             
    //         }
    //     }
    // }
}