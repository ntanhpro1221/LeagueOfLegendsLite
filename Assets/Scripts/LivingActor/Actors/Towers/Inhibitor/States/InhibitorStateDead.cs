using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public static partial class InhibitorStateDead {
    public const float RespawnTime = 10f;
    
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    public partial struct Exit : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<EnumIndexData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            state.CompleteDependency();

            var curTick  = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
            var healthId = SystemAPI.GetSingleton<EnumIndexData>().StatsType[StatsType.Health];

            foreach (var (
                    filter
                  , sharedState
                    , transition
                  , data)
                in SystemAPI.Query<
                    StateFilterAspect
                  , ActorSharedStateAspect
                    , TransitionStateAspectRW
                  , UpdateAspect>()) {

                // DEAD_2_IDLE STATE
                if (curTick.IsNewerThan(data.RespawnTick)) { // It's tick to respawn
                    sharedState.SetDead2Idle();
                    transition.HardCutAnim = false;
                }
                else continue;

                filter.MarkExitExecuted();

                data.CurHealth = data.MaxHealth(healthId); // Respawn with full health
            }
        }

        private readonly partial struct UpdateAspect : IAspect {
            private readonly RefRW<HealthData>    _HealthData;
            private readonly RefRO<DeadStateData> _DeadStateData;

            [ReadOnly] private readonly DynamicBuffer<StatsBuffer> _Stats;

            public ref float_Q3    CurHealth   => ref _HealthData.ValueRW.value;
            public     NetworkTick RespawnTick => _DeadStateData.ValueRO.respawnAtTick;

            public float_Q3 MaxHealth(int healthId) => _Stats[healthId].value;
        }
    }

    [UpdateInGroup(typeof(StateEnterSystemGroup))]
    public partial struct Enter : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ClientServerTickRate>();
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (_, data) in SystemAPI.Query<
                StateFilterAspect
              , UpdateAspect>()) {
                data.CurAnim = SharedAnimKey.Dead;

                data.RespawnAtTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick.WithDeltaTime(
                    RespawnTime, SystemAPI.GetSingleton<ClientServerTickRate>().SimulationTickRate);
            }
        }

        private readonly partial struct UpdateAspect : IAspect {
            private readonly RefRW<DeadStateData>  _DeadStateData;
            private readonly RefRW<SharedAnimData> _AnimData;

            public ref NetworkTick   RespawnAtTick => ref _DeadStateData.ValueRW.respawnAtTick;
            public ref SharedAnimKey CurAnim       => ref _AnimData.ValueRW.curAnim;
        }
    }
}

public static partial class InhibitorStateDead {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<InhibitorTag, DeadState> {
            private readonly RefRO<InhibitorTag> _identity;
            private readonly RefRO<Simulate>     _simulate;

            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<DeadState>         _curStateEnable;

            RefRO<InhibitorTag> IStateAspect<InhibitorTag, DeadState>.Identity => _identity;
            RefRO<Simulate> IStateAspect<InhibitorTag, DeadState>.    Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitAspect<InhibitorTag, DeadState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<DeadState> IStateExitAspect<InhibitorTag, DeadState>.        CurStateEnable    => _curStateEnable;

            public void MarkExitExecuted() => _stateNotExitedYet.ValueRW = _curStateEnable.ValueRW = false;
        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<InhibitorTag, DeadState> {
            private readonly RefRO<InhibitorTag> _identity;
            private readonly RefRO<DeadState>    _curState;
            private readonly RefRO<Simulate>     _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<InhibitorTag> IStateAspect<InhibitorTag, DeadState>.Identity => _identity;
            RefRO<DeadState> IStateAspect<InhibitorTag, DeadState>.   CurState => _curState;
            RefRO<Simulate> IStateAspect<InhibitorTag, DeadState>.    Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<InhibitorTag, DeadState>.StateRequireEnter => _stateRequireEnter;
        }
    }
}