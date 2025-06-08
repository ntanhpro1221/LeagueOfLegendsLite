using NGDtuanh.Entities.StateMachine;
using Pathfinding;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

public static partial class MonsterStateDead {
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

                // IDLE STATE
                if (data.RespawnTick != NetworkTick.Invalid  // Not waiting for all camp dead
                 && curTick.IsNewerThan(data.RespawnTick)) { // It's time to respawn
                    if (data.CanRespawn) sharedState.SetIdle();
                    else data.Destroy();
                } else continue;

                filter.MarkExitExecuted();
                if (data.CanRespawn) {
                    data.LocalTrans = initTrans[filter.Id][data.TeamType][0].ToLocTrans_Directly(); // Respawn at init pos
                    data.CurHealth  = data.MaxHealth;                                               // Respawn with full health
                    data.EnableMove();                                                              // enable move
                    data.TrySpawnExtraMonster();                                                    // Try spawn extra monster
                    select_highlight_healthBar.EnableAll();                                         // enable select and highlight and health bar
                }
            }
        }

        private readonly partial struct UpdateAspect : IAspect {
            private readonly RefRW<HealthData>         _HealthData;
            private readonly RefRW<LocalTransform>     _LocalTrans;
            private readonly RefRO<JungleTeamTypeData> _TeamType;
            private readonly RefRO<DeadStateData>      _DeadStateData;
            private readonly FixablePosSetterAspect    _FixSetter;

            [ReadOnly] private readonly DynamicBuffer<StatsBuffer> _Stats;

            [Optional] private readonly RefRW<MonsterLeaderData>          _LeaderData;
            [Optional] private readonly RefRO<MonsterExtraBufferCount>    _ExtraCount;
            [Optional] private readonly EnabledRefRW<MonsterExtraTrigger> _ExtraTrigger;
            [Optional] private readonly EnabledRefRO<MonsterCanRespawn>   _CanRespawn;
            [Optional] private readonly EnabledRefRW<NetworkDestroyedTag> _Destroyed;

            public              bool           CanRespawn  => _CanRespawn.ValueRO;
            public ref          LocalTransform LocalTrans  => ref _LocalTrans.ValueRW;
            public ref          float_Q3       CurHealth   => ref _HealthData.ValueRW.value;
            public ref readonly TeamType       TeamType    => ref _TeamType.ValueRO.team;
            public ref readonly NetworkTick    RespawnTick => ref _DeadStateData.ValueRO.respawnAtTick;

            public float_Q3 MaxHealth    => _Stats[StatsId.Health].value;
            public void     EnableMove() => _FixSetter.Release();
            public void     Destroy()    => _Destroyed.ValueRW = true;

            public void TrySpawnExtraMonster() {
                if (_ExtraTrigger.IsValid) {
                    _ExtraTrigger.ValueRW              =  true;
                    _LeaderData.ValueRW.underlingCount += _ExtraCount.ValueRO.Count;
                }
            }
        }
    }

    [UpdateInGroup(typeof(StateEnterSystemGroup))]
    public partial struct Enter : ISystem {
        private ComponentLookup<MonsterLeaderData>     leaderLookup;
        private BufferLookup<MonsterMyUnderlingBuffer> underlingBufferLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
            leaderLookup = SystemAPI.GetComponentLookup<MonsterLeaderData>(
                isReadOnly: false);
            underlingBufferLookup = SystemAPI.GetBufferLookup<MonsterMyUnderlingBuffer>(
                isReadOnly: false);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            leaderLookup.Update(ref state);
            underlingBufferLookup.Update(ref state);

            foreach (var (_, data, select_highlight_healthBar) in SystemAPI.Query<
                StateFilterAspect
              , UpdateAspect
              , Select_Highlight_HealthBarAspect>()) {
                data.CurAnim = SharedAnimKey.Dead;

                data.SetNullRespawnTick();
                data.DisableMoveAndAvoidance();
                data.ResetLeashState();
                data.UpdateUnderlingCountAndTryDivide(
                    leaderLookup
                  , underlingBufferLookup);
                select_highlight_healthBar.DisableAll();
            }
        }

        private readonly partial struct UpdateAspect : IAspect {
            private readonly RefRW<DeadStateData>   _DeadStateData;
            private readonly RefRW<SharedAnimData>  _AnimData;
            private readonly RefRO<LocalTransform>  _LocTrans;
            private readonly FixablePosSetterAspect _FixSetter;

            private readonly MonsterCampRootRO _CampRoot;

            [Optional] private readonly RefRO<MonsterDivideBufferCount>               _DivideCount;
            [Optional] private readonly EnabledRefRW<MonsterDivideTrigger>            _DivideTrigger;
            [Optional] private readonly EnabledRefRW<AutoFollowTarget_FollowerEntity> _AutoFollow;
            [Optional] private readonly EnabledRefRW<MonsterLeashAnchor>              _AnchorTrigger;
            [Optional] private readonly EnabledRefRW<MonsterLeashDisabling>           _UnleashTrigger;

            public ref SharedAnimKey CurAnim => ref _AnimData.ValueRW.curAnim;

            public void SetNullRespawnTick() {
                _DeadStateData.ValueRW.respawnAtTick = NetworkTick.Invalid;
            }

            public void DisableMoveAndAvoidance() {
                _FixSetter.FixAt(_LocTrans.ValueRO.Position, false);

                _AutoFollow.ValueRW = false;
            }

            public void ResetLeashState() {
                _AnchorTrigger.ValueRW  = false;
                _UnleashTrigger.ValueRW = false;
            }

            public void UpdateUnderlingCountAndTryDivide(
                in ComponentLookup<MonsterLeaderData>     leaderLookup
              , in BufferLookup<MonsterMyUnderlingBuffer> underlingBufferLookup) {
                RefRW<MonsterLeaderData> leaderData = default;

                if (_CampRoot.TryGetRoot(out var root)) {
                    // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
                    leaderData = leaderLookup.GetRefRW(root);

                    if (root != _CampRoot.MyEntity) {
                        --leaderData.ValueRW.underlingCount;
                        underlingBufferLookup[root].Remove(_CampRoot.MyEntity);
                    }
                }

                if (_DivideTrigger.IsValid) {
                    _DivideTrigger.ValueRW = true;
                    if (leaderData.IsValid)
                        leaderData.ValueRW.underlingCount += _DivideCount.ValueRO.Count;
                }
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
                if (data.HaveUnderling
                 || data.DividingMonster
                 || data.SpawningExtraMonster
                 || data.AlreadyCalculatedRespawnTick)
                    continue;

                data.SetRespawnTick(curTick);
            }
        }

        private readonly partial struct UpdateAspect : IAspect {
            private readonly RefRW<DeadStateData>        _DeadData;
            private readonly RefRO<MonsterControlFactor> _ControlFactor;

            [Optional] private readonly RefRO<MonsterLeaderData>           _LeaderData;
            [Optional] private readonly EnabledRefRO<MonsterDivideTrigger> _DivideTrigger;
            [Optional] private readonly EnabledRefRO<MonsterExtraTrigger>  _ExtraTrigger;

            public bool HaveUnderling =>
                _LeaderData.IsValid
             && _LeaderData.ValueRO.underlingCount != 0;

            public bool DividingMonster =>
                _DivideTrigger.IsValid
             && _DivideTrigger.ValueRO;

            public bool SpawningExtraMonster =>
                _ExtraTrigger.IsValid
             && _ExtraTrigger.ValueRO;

            public bool AlreadyCalculatedRespawnTick =>
                _DeadData.ValueRO.respawnAtTick != NetworkTick.Invalid;

            public void SetRespawnTick(in NetworkTick curTick) {
                _DeadData.ValueRW.respawnAtTick = curTick.WithBonusTick(_ControlFactor.ValueRO.respawnCDTick);
            }
        }
    }
}

public static partial class MonsterStateDead {
    public partial struct Exit {
        public struct InheritTag : IStateInheritTag<MonsterTag, DeadState> { }

        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<MonsterTag, DeadState>.RequireInherit<Update.InheritTag> {
            private readonly RefRO<MonsterTag>        _identity;
            private readonly RefRO<Simulate>          _simulate;
            private readonly RefRO<Update.InheritTag> _inheritTag;

            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<DeadState>         _curStateEnable;

            RefRO<MonsterTag> IStateAspect<MonsterTag, DeadState>.                               Identity   => _identity;
            RefRO<Simulate> IStateAspect<MonsterTag, DeadState>.                                 Simulate   => _simulate;
            RefRO<Update.InheritTag> IStateInheritable<MonsterTag, DeadState, Update.InheritTag>.InheritTag => _inheritTag;

            EnabledRefRW<StateNotExitedYet> IStateExitAspect<MonsterTag, DeadState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<DeadState> IStateExitAspect<MonsterTag, DeadState>.        CurStateEnable    => _curStateEnable;

            public void MarkExitExecuted() => _stateNotExitedYet.ValueRW = _curStateEnable.ValueRW = false;

            public MonsterId Id => _identity.ValueRO.id;
        }
    }

    public partial struct Enter {
        public struct InheritTag : IStateInheritTag<MonsterTag, DeadState> { }

        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<MonsterTag, DeadState>.RequireInherit<InheritTag> {
            private readonly RefRO<MonsterTag> _identity;
            private readonly RefRO<DeadState>  _curState;
            private readonly RefRO<Simulate>   _simulate;
            private readonly RefRO<InheritTag> _inheritTag;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<MonsterTag> IStateAspect<MonsterTag, DeadState>.                 Identity   => _identity;
            RefRO<DeadState> IStateAspect<MonsterTag, DeadState>.                  CurState   => _curState;
            RefRO<Simulate> IStateAspect<MonsterTag, DeadState>.                   Simulate   => _simulate;
            RefRO<InheritTag> IStateInheritable<MonsterTag, DeadState, InheritTag>.InheritTag => _inheritTag;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<MonsterTag, DeadState>.StateRequireEnter => _stateRequireEnter;
        }
    }

    public partial struct Update {
        public struct InheritTag : IStateInheritTag<MonsterTag, DeadState> { }

        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<MonsterTag, DeadState>.RequireInherit<InheritTag> {
            private readonly RefRO<MonsterTag> _identity;
            private readonly RefRO<DeadState>  _curState;
            private readonly RefRO<Simulate>   _simulate;
            private readonly RefRO<InheritTag> _inheritTag;

            RefRO<MonsterTag> IStateAspect<MonsterTag, DeadState>.                 Identity   => _identity;
            RefRO<DeadState> IStateAspect<MonsterTag, DeadState>.                  CurState   => _curState;
            RefRO<Simulate> IStateAspect<MonsterTag, DeadState>.                   Simulate   => _simulate;
            RefRO<InheritTag> IStateInheritable<MonsterTag, DeadState, InheritTag>.InheritTag => _inheritTag;
        }
    }
}