using NGDtuanh.Entities.StateMachine;
using Pathfinding;
using Pathfinding.ECS;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

public static partial class MinionStateDead {
    // Exit to destroy minion
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
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

            var curTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            foreach (var (
                    _
                  , sharedState
                  , data)
                in SystemAPI.Query<
                    StateFilterAspect
                  , ActorSharedStateAspect
                  , UpdateAspect>()) {

                if (curTick.IsNewerThan(data.RespawnTick)) // It's tick to DISAPPEAR
                    sharedState.SetIdle();
                else continue;

                data.Destroy();
            }
        }

        private readonly partial struct UpdateAspect : IAspect {
            private readonly RefRO<DeadStateData> _DeadStateData;

            [Optional] private readonly EnabledRefRW<NetworkDestroyedTag> _NetworkDestroyed;

            public NetworkTick RespawnTick => _DeadStateData.ValueRO.respawnAtTick;
            public void        Destroy()   => _NetworkDestroyed.ValueRW = true;
        }
    }

    [UpdateInGroup(typeof(StateEnterSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct Enter : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (_, data, select_highlight_healthBar, anim) in SystemAPI.Query<
                StateFilterAspect
              , UpdateAspect
              , Select_Highlight_HealthBarAspect
              , RefRO<SharedAnimData>>()) {
                data.CurAnim = SharedAnimKey.Dead;

                // Respawn here means disappear
                data.RespawnAtTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick.WithBonusTick(
                    anim.ValueRO.AnimLengthTicks[SharedAnimKey.Dead]);
                data.DisableMove();
                select_highlight_healthBar.DisableAll();
            }
        }

        private readonly partial struct UpdateAspect : IAspect {
            private readonly RefRW<DeadStateData>   _DeadStateData;
            private readonly RefRW<SharedAnimData>  _AnimData;
            private readonly RefRO<LocalTransform>  _LocTrans;
            private readonly FixablePosSetterAspect _FixSetter;

            [Optional] private readonly EnabledRefRW<AutoFollowTarget_FollowerEntity> _AutoFollow;

            public ref NetworkTick   RespawnAtTick => ref _DeadStateData.ValueRW.respawnAtTick;
            public ref SharedAnimKey CurAnim       => ref _AnimData.ValueRW.curAnim;

            public void DisableMove() {
                _FixSetter.FixAt(_LocTrans.ValueRO.Position);
                _AutoFollow.ValueRW = false;
            }
        }
    }
}

public static partial class MinionStateDead {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<MinionTag, DeadState> {
            private readonly RefRO<MinionTag> _identity;
            private readonly RefRO<Simulate>  _simulate;

            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<DeadState>         _curStateEnable;

            RefRO<MinionTag> IStateAspect<MinionTag, DeadState>.Identity => _identity;
            RefRO<Simulate> IStateAspect<MinionTag, DeadState>. Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitAspect<MinionTag, DeadState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<DeadState> IStateExitAspect<MinionTag, DeadState>.        CurStateEnable    => _curStateEnable;

            public void MarkExitExecuted() => _stateNotExitedYet.ValueRW = _curStateEnable.ValueRW = false;
        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<MinionTag, DeadState> {
            private readonly RefRO<MinionTag> _identity;
            private readonly RefRO<DeadState> _curState;
            private readonly RefRO<Simulate>  _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<MinionTag> IStateAspect<MinionTag, DeadState>.Identity => _identity;
            RefRO<DeadState> IStateAspect<MinionTag, DeadState>.CurState => _curState;
            RefRO<Simulate> IStateAspect<MinionTag, DeadState>. Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<MinionTag, DeadState>.StateRequireEnter => _stateRequireEnter;
        }
    }
}