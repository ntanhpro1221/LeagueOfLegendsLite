using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

public static partial class ChampionStateIdle {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
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
              , commonChamp
              , attackData
                ) in SystemAPI.Query<
                StateFilterAspect
              , CommonExitStateAspect
              , CommonExitStateAspect_Champion
              , RefRO<AttackStateData>>()) {

                // DEAD STATE
                if (common.Health.IsDead) // Run out of health.
                    common.State.SetDead();

                // ITEM ANALYZING STATE
                else if (
                    // Not have disabling activate item CC.
                    common.CC.Disable.ActiveItem == 0
                    // Have request.
                 && commonChamp.ItemRequest.haveRequestNewState)
                    common.State.SetItemActiveAnalyzing();

                // MOVE STATE
                else if (
                    // Not have disabling move CC.
                    common.CC.Disable.Move == 0 && (
                        // Need move to target.
                        common.Target.NeedMoveToTarget(selectLookup, locTransLookup, statsLookup)
                        // Have move request.
                     || commonChamp.Input.MoveEvent_WithData))
                    common.State.SetMove();

                // ATTACK STATE
                else if (
                    // Not have disabling move CC.
                    common.CC.Disable.Attack == 0
                    // Have target.
                 && common.Target.IsTargetExists(selectLookup)
                    // Attack cool down done.
                 && attackData.ValueRO.IsCooldownDone(curTick))
                    common.State.SetAttack();

                else continue;

                IStateExitFunc<IdleState>.MarkExitExecuted(filter);
            }
        }
    }

    [UpdateInGroup(typeof(StateEnterSystemGroup))]
    public partial struct Enter : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (_, anim) in SystemAPI.Query<
                StateFilterAspect
              , SharedAnimAspect>()) {
                anim.SetAnim(SharedAnimKey.Idle);
            }
        }
    }

    [UpdateInGroup(typeof(StateUpdateSystemGroup))]
    public partial struct Update : ISystem {
        [ReadOnly] private ComponentLookup<Selectable>     selectLookup;
        [ReadOnly] private ComponentLookup<LocalTransform> locTransLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            selectLookup = SystemAPI.GetComponentLookup<Selectable>(
                isReadOnly: true);
            locTransLookup = SystemAPI.GetComponentLookup<LocalTransform>(
                isReadOnly: true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            selectLookup.Update(ref state);
            locTransLookup.Update(ref state);

            // ROTATE TO TARGET
            foreach (var (_, rotationData, target, locTrans) in SystemAPI
                .Query<
                    StateFilterAspect
                  , RefRW<RotationData>
                  , AimedTargetAspectRO
                  , RefRO<LocalTransform>>())
                if (target.IsTargetExists(selectLookup))
                    rotationData.ValueRW.RotateTo((
                        locTransLookup[target.Target].Position
                      - locTrans.ValueRO.Position
                    ).Quantizate3().xz);
        }
    }
}

public static partial class ChampionStateIdle {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<ChampionTag, IdleState> {
            private readonly RefRO<ChampionTag> _identity;
            private readonly RefRO<Simulate>    _simulate;

            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<IdleState>         _curStateEnable;

            RefRO<ChampionTag> IStateAspect<ChampionTag, IdleState>.Identity => _identity;
            RefRO<Simulate> IStateAspect<ChampionTag, IdleState>.   Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitFunc<IdleState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<IdleState> IStateExitFunc<IdleState>.        CurStateEnable    => _curStateEnable;
        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<ChampionTag, IdleState> {
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

    public partial struct Update {
        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<ChampionTag, IdleState> {
            private readonly RefRO<ChampionTag> _identity;
            private readonly RefRO<IdleState>   _curState;
            private readonly RefRO<Simulate>    _simulate;

            RefRO<ChampionTag> IStateAspect<ChampionTag, IdleState>.Identity => _identity;
            RefRO<IdleState> IStateAspect<ChampionTag, IdleState>.  CurState => _curState;
            RefRO<Simulate> IStateAspect<ChampionTag, IdleState>.   Simulate => _simulate;
        }
    }
}