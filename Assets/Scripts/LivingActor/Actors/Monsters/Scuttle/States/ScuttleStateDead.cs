using NGDtuanh.Entities.StateMachine;
using Pathfinding;
using Pathfinding.ECS;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

public static partial class ScuttleStateDead {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    public partial struct Exit : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<InitTransformData>();
            state.RequireForUpdate<EnumIndexData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            state.CompleteDependency();

            var     curTick   = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
            var     healthId  = SystemAPI.GetSingleton<EnumIndexData>().StatsType[StatsType.Health];
            ref var initTrans = ref SystemAPI.GetSingleton<InitTransformData>().Monster.Value;

            foreach (var (
                    filter
                  , sharedState
                  , data
                  , select_highlight_healthBar)
                in SystemAPI.Query<
                    StateFilterAspect
                  , ActorSharedStateAspect
                  , UpdateAspect
                  , Select_Highlight_HealthBarAspect>()) {

                // MOVE STATE
                if (data.RespawnTick != NetworkTick.Invalid // Waiting for something
                 && curTick.IsNewerThan(data.RespawnTick)) // It's time to respawn
                    sharedState.SetMove();
                else continue;

                filter.MarkExitExecuted();

                data.LocalTrans = initTrans[filter.Id][data.TeamType][0].ToLocTrans_Directly(); // Respawn at init pos
                data.CurHealth  = data.MaxHealth(healthId);                                     // Respawn with full health
                data.EnableMove();                                                              // enable move
                select_highlight_healthBar.EnableAll();                                         // enable select and highlight and health bar
            }
        }

        private readonly partial struct UpdateAspect : IAspect {
            private readonly RefRW<HealthData>         _HealthData;
            private readonly RefRW<LocalTransform>     _LocalTrans;
            private readonly RefRO<JungleTeamTypeData> _TeamType;
            private readonly RefRO<DeadStateData>      _DeadStateData;
            private readonly RefRW<MovementSettings>   _MoveSetting;

            [ReadOnly] private readonly DynamicBuffer<StatsBuffer> _Stats;

            public ref          LocalTransform LocalTrans  => ref _LocalTrans.ValueRW;
            public ref          float_Q3       CurHealth   => ref _HealthData.ValueRW.value;
            public ref readonly TeamType       TeamType    => ref _TeamType.ValueRO.team;
            public ref readonly NetworkTick    RespawnTick => ref _DeadStateData.ValueRO.respawnAtTick;

            public float_Q3 MaxHealth(int healthId) => _Stats[healthId].value;
            public void     EnableMove()            => _MoveSetting.ValueRW.isStopped = false;
        }
    }

    [UpdateInGroup(typeof(StateEnterSystemGroup))]
    public partial struct Enter : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (_, data, select_highlight_healthBar) in SystemAPI.Query<
                StateFilterAspect
              , UpdateAspect
              , Select_Highlight_HealthBarAspect>()) {
                data.CurAnim = SharedAnimKey.Dead;

                data.SetNullRespawnTick();
                data.DisableMove();
                select_highlight_healthBar.DisableAll();
            }
        }

        private readonly partial struct UpdateAspect : IAspect {
            private readonly RefRW<DeadStateData>    _DeadStateData;
            private readonly RefRW<SharedAnimData>   _AnimData;
            private readonly RefRW<MovementSettings> _MoveSetting;

            public ref SharedAnimKey CurAnim => ref _AnimData.ValueRW.curAnim;

            public void SetNullRespawnTick() {
                _DeadStateData.ValueRW.respawnAtTick = NetworkTick.Invalid;
            }

            public void DisableMove() {
                _MoveSetting.ValueRW.isStopped = true;
            }
        }
    }

    public partial struct Update : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            state.CompleteDependency();

            var curTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            foreach (var (_, data) in SystemAPI.Query<
                StateFilterAspect
              , UpdateAspect>()) {
                if (data.AlreadyCalculatedRespawnTick)
                    continue;

                data.SetRespawnTick(curTick);
            }
        } 

        private readonly partial struct UpdateAspect : IAspect {
            private readonly RefRW<DeadStateData>        _DeadData;
            private readonly RefRO<MonsterControlFactor> _ControlFactor;

            public bool AlreadyCalculatedRespawnTick =>
                _DeadData.ValueRO.respawnAtTick != NetworkTick.Invalid;

            public void SetRespawnTick(in NetworkTick curTick) {
                _DeadData.ValueRW.respawnAtTick = curTick.WithBonusTick(_ControlFactor.ValueRO.respawnCDTick);
            }
        }
    }
}

public static partial class ScuttleStateDead {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<ScuttleTag, DeadState> {
            private readonly RefRO<ScuttleTag> _identity;
            private readonly RefRO<Simulate>   _simulate;

            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<DeadState>         _curStateEnable;

            RefRO<ScuttleTag> IStateAspect<ScuttleTag, DeadState>.Identity => _identity;
            RefRO<Simulate> IStateAspect<ScuttleTag, DeadState>.  Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitAspect<ScuttleTag, DeadState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<DeadState> IStateExitAspect<ScuttleTag, DeadState>.        CurStateEnable    => _curStateEnable;

            public void MarkExitExecuted() => _stateNotExitedYet.ValueRW = _curStateEnable.ValueRW = false;

            public MonsterId Id => MonsterId.Scuttle;
        }
    }

    public partial struct Enter {

        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<ScuttleTag, DeadState> {
            private readonly RefRO<ScuttleTag> _identity;
            private readonly RefRO<DeadState>  _curState;
            private readonly RefRO<Simulate>   _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<ScuttleTag> IStateAspect<ScuttleTag, DeadState>.Identity => _identity;
            RefRO<DeadState> IStateAspect<ScuttleTag, DeadState>. CurState => _curState;
            RefRO<Simulate> IStateAspect<ScuttleTag, DeadState>.  Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<ScuttleTag, DeadState>.StateRequireEnter => _stateRequireEnter;
        }
    }

    public partial struct Update {
        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<ScuttleTag, DeadState> {
            private readonly RefRO<ScuttleTag> _identity;
            private readonly RefRO<DeadState>  _curState;
            private readonly RefRO<Simulate>   _simulate;

            RefRO<ScuttleTag> IStateAspect<ScuttleTag, DeadState>.Identity => _identity;
            RefRO<DeadState> IStateAspect<ScuttleTag, DeadState>. CurState => _curState;
            RefRO<Simulate> IStateAspect<ScuttleTag, DeadState>.  Simulate => _simulate;
        }
    }
}