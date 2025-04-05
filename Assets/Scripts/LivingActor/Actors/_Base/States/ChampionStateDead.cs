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
            state.RequireForUpdate<EnumIndexData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var data in SystemAPI.Query<UpdateAspect>()) {
                var curTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

                // IDLE STATE
                if (curTick.IsNewerThan(data.RespawnTick)) // It's tick to respawn
                    data.IdleState.ValueRW = true;
                else continue;

                data.MarkExitExecuted();

                ref var champInitTrans = ref SystemAPI.GetSingleton<InitTransformData>().Champion.Value;
                data.LocalTrans          = champInitTrans[data.TeamType][0].ToLocalTransform_Directly(); // Respawn at init pos
                data.MoveTarget          = data.LocalTrans.Position.Quantizate3();                       // Reset target pos at init pos
                data.CurHealth           = data.GetMaxHealth(SystemAPI.GetSingleton<EnumIndexData>());   // Respawn with full health
                data.MoveEnabled.ValueRW = true;                                                         // enable move
            }
        }

        private readonly partial struct UpdateAspect {
            private readonly DynamicBuffer<StatsBuffer> _Stats;

            private readonly RefRW<PlayerInputData> _MoveInput;
            private readonly RefRW<HealthData>      _HealthData;
            private readonly RefRW<LocalTransform>  _LocalTrans;

            private readonly RefRO<TeamTypeData>  _TeamType;
            private readonly RefRO<DeadStateData> _DeadStateData;

            [Optional] public readonly EnabledRefRW<MoveData>  MoveEnabled;
            [Optional] public readonly EnabledRefRW<IdleState> IdleState;

            public ref LocalTransform LocalTrans  => ref _LocalTrans.ValueRW;
            public ref float_Q3       CurHealth   => ref _HealthData.ValueRW.value;
            public ref float3_Q3      MoveTarget  => ref _MoveInput.ValueRW.targetLocalPos;
            public     TeamType       TeamType    => _TeamType.ValueRO.teamType;
            public     NetworkTick    RespawnTick => _DeadStateData.ValueRO.respawnAtTick;

            public float_Q3 GetMaxHealth(EnumIndexData enumData)
                => _Stats[enumData.StatsType[StatsType.Health]].value;
        }

        private readonly partial struct UpdateAspect : IAspect, IStateExitAspect<ChampionTag, DeadState> {
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

    [UpdateInGroup(typeof(StateEnterSystemGroup))]
    public partial struct Enter : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ClientServerTickRate>();
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var data in SystemAPI.Query<UpdateAspect>()) {
                data.CurAnim = SharedAnimKey.Dead;

                data.RespawnAtTick = GameRuleHelpers.CalcRespawnTick(
                    SystemAPI.GetSingletonBuffer<BaseRespawnWaitTimeBuffer>()
                  , SystemAPI.GetSingleton<NetworkTime>()
                  , SystemAPI.GetSingleton<ClientServerTickRate>().SimulationTickRate
                  , data.CurLevel);
                data.MoveEnabled.ValueRW = false;
            }
        }

        private readonly partial struct UpdateAspect {
            private readonly RefRW<DeadStateData> _DeadStateData;
            private readonly RefRW<AnimData>      _AnimData;

            private readonly RefRO<LevelData> _LevelData;

            [Optional] public readonly EnabledRefRW<MoveData>            MoveEnabled;

            public ref NetworkTick   RespawnAtTick => ref _DeadStateData.ValueRW.respawnAtTick;
            public ref SharedAnimKey CurAnim       => ref _AnimData.ValueRW.curAnim;

            public int CurLevel => _LevelData.ValueRO.curLevel;
        }

        private readonly partial struct UpdateAspect : IAspect, IStateEnterAspect<ChampionTag, DeadState> {
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
    //                   , DeadState
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
    //                   , DeadState
    //                   , Simulate>()) {
    //             
    //         }
    //     }
    // }
}