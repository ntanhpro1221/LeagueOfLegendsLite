using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public static partial class ChampionStateIdle {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    public partial struct Exit : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var data in SystemAPI.Query<UpdateAspect>()) {
                // DEAD STATE
                if (data.CurHealth <= 0) // RUN OUT OF HEALTH
                    data.DeadStateEnable.ValueRW = true;
                // MOVE STATE
                else if (data.SumVelocityXZ > float_Q3.Epsilon) // HAVE VELOCITY
                    data.MoveStateEnable.ValueRW = true;
                else continue;

                data.MarkExitExecuted();
            }
        }

        private readonly partial struct UpdateAspect {
            private readonly RefRO<HealthData>      _HealthData;
            private readonly RefRO<PhysicsVelocity> _PhysicsVelocity;

            [Optional] public readonly EnabledRefRW<DeadState> DeadStateEnable;
            [Optional] public readonly EnabledRefRW<MoveState> MoveStateEnable;

            public float_Q3 CurHealth => _HealthData.ValueRO.value;

            public float SumVelocityXZ =>
                math.abs(_PhysicsVelocity.ValueRO.Linear.x)
              + math.abs(_PhysicsVelocity.ValueRO.Linear.z);
        }

        private readonly partial struct UpdateAspect : IAspect, IStateExitAspect<ChampionTag, IdleState> {
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

    [UpdateInGroup(typeof(StateEnterSystemGroup))]
    public partial struct Enter : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var data in SystemAPI.Query<UpdateAspect>()) {
                data.CurAnim = SharedAnimKey.Idle;
            }
        }

        private readonly partial struct UpdateAspect {
            private readonly RefRW<AnimData> _AnimData;

            public ref SharedAnimKey CurAnim => ref _AnimData.ValueRW.curAnim;
        }

        private readonly partial struct UpdateAspect : IAspect, IStateEnterAspect<ChampionTag, IdleState> {
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
    //                   , IdleState
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
    //                   , IdleState
    //                   , Simulate>()) {
    //             
    //         }
    //     }
    // }
}