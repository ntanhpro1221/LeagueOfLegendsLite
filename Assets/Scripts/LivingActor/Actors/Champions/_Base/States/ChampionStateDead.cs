using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

public static partial class ChampionStateDead {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    public partial struct Exit : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<InitTransformData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            state.CompleteDependency();

            var     curTick   = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
            ref var initTrans = ref SystemAPI.GetSingleton<InitTransformData>().Champion.Value;

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

                // IDLE STATE
                if (curTick.IsNewerThan(data.RespawnTick)) // It's tick to respawn
                    sharedState.SetIdle();
                else continue;

                filter.MarkExitExecuted();

                data.LocalTrans = initTrans[data.TeamType][0].ToLocTrans_Directly(); // Respawn at init pos
                data.MoveRequester.SyncFromLocTrans(data.LocalTrans);                // Reset target pos at init pos
                data.CurHealth = data.MaxHealth;                                     // Respawn with full health
                data.EnableMove();                                                   // enable move
                data.RequireInputReset();                                            // require input reset
                select_highlight_healthBar.EnableAll();                              // enable select and highlight and health bar
            }
        }

        private readonly partial struct UpdateAspect : IAspect {
            private readonly RefRW<HealthData>     _HealthData;
            private readonly RefRW<LocalTransform> _LocalTrans;
            private readonly RefRO<TeamTypeData>   _TeamType;
            private readonly RefRO<DeadStateData>  _DeadStateData;
            private readonly RefRO<StatsData>      _Stats;

            [Optional] private readonly EnabledRefRW<MoveableTag>          _Moveable;
            [Optional] private readonly EnabledRefRW<PlayerInputResetting> _InputReset;

            public readonly MoveRequesterAspect MoveRequester;

            public              float_Q3       MaxHealth   => _Stats.ValueRO.data.Health;
            public ref          LocalTransform LocalTrans  => ref _LocalTrans.ValueRW;
            public ref          float_Q3       CurHealth   => ref _HealthData.ValueRW.value;
            public ref readonly TeamType       TeamType    => ref _TeamType.ValueRO.team;
            public ref readonly NetworkTick    RespawnTick => ref _DeadStateData.ValueRO.respawnAtTick;

            public void EnableMove()        => _Moveable.ValueRW = true;
            public void RequireInputReset() => _InputReset.ValueRW = true;
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
            foreach (var (_, data, select_highlight_healthBar) in SystemAPI.Query<
                StateFilterAspect
              , UpdateAspect
              , Select_Highlight_HealthBarAspect>()) {
                data.CurAnim = SharedAnimKey.Dead;

                data.RespawnAtTick = GameHelpers.CalcRespawnTick_Champion(
                    SystemAPI.GetSingletonBuffer<BaseRespawnWaitTimeBuffer>()
                  , SystemAPI.GetSingleton<NetworkTime>()
                  , SystemAPI.GetSingleton<ClientServerTickRate>().SimulationTickRate
                  , data.CurLevel);
                data.DisableMove();
                select_highlight_healthBar.DisableAll();
            }
        }

        private readonly partial struct UpdateAspect : IAspect {
            private readonly RefRW<DeadStateData>  _DeadStateData;
            private readonly RefRW<SharedAnimData> _AnimData;

            private readonly RefRO<LevelData> _LevelData;

            [Optional] private readonly EnabledRefRW<MoveableTag> _Moveable;

            public ref NetworkTick   RespawnAtTick => ref _DeadStateData.ValueRW.respawnAtTick;
            public ref SharedAnimKey CurAnim       => ref _AnimData.ValueRW.curAnim;

            public int CurLevel => _LevelData.ValueRO.curLevel;

            public void DisableMove() => _Moveable.ValueRW = false;
        }
    }
}

public static partial class ChampionStateDead {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<ChampionTag, DeadState> {
            private readonly RefRO<ChampionTag> _identity;
            private readonly RefRO<Simulate>    _simulate;

            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<DeadState>         _curStateEnable;

            RefRO<ChampionTag> IStateAspect<ChampionTag, DeadState>.Identity => _identity;
            RefRO<Simulate> IStateAspect<ChampionTag, DeadState>.   Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitAspect<ChampionTag, DeadState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<DeadState> IStateExitAspect<ChampionTag, DeadState>.        CurStateEnable    => _curStateEnable;

            public void MarkExitExecuted() => _stateNotExitedYet.ValueRW = _curStateEnable.ValueRW = false;
        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<ChampionTag, DeadState> {
            private readonly RefRO<ChampionTag> _identity;
            private readonly RefRO<DeadState>   _curState;
            private readonly RefRO<Simulate>    _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<ChampionTag> IStateAspect<ChampionTag, DeadState>.Identity => _identity;
            RefRO<DeadState> IStateAspect<ChampionTag, DeadState>.  CurState => _curState;
            RefRO<Simulate> IStateAspect<ChampionTag, DeadState>.   Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<ChampionTag, DeadState>.StateRequireEnter => _stateRequireEnter;
        }
    }
}