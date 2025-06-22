using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

public static partial class ChampionStateAttack {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    public partial struct Exit : ISystem {
        [ReadOnly] private ComponentLookup<Selectable>     selectLookup;
        [ReadOnly] private ComponentLookup<LocalTransform> locTransLookup;
        [ReadOnly] private ComponentLookup<StatsData>      statsLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
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

            foreach (var (
                filter
              , common
              , commonChamp
              , attackData
                ) in SystemAPI.Query<
                StateFilterAspect
              , CommonExitStateAspect
              , CommonExitStateAspect_Champion
              , RefRW<AttackStateData>>()) {

                // DEAD STATE
                if (common.Health.IsDead) // Run out of health.
                    common.State.SetDead();

                // ITEM ANALYZING STATE
                else if (
                    // Not have disabling activate item CC.
                    common.CC.Disable.ActiveItem == 0
                    // Have request.
                 && commonChamp.ItemRequest.haveRequest)
                    common.State.SetItemActiveAnalyzing();

                // MOVE STATE
                else if (
                    // Not have disabling move CC.
                    common.CC.Disable.Move == 0 && (
                        // Need move to target and already perform attack yet.
                        (attackData.ValueRO.isAttacked && common.Target.NeedMoveToTarget(selectLookup, locTransLookup, statsLookup))
                        // Have move request.
                     || commonChamp.Input.MoveEvent_WithData))
                    common.State.SetMove();

                // IDLE STATE
                else if (
                    // Have disabling attack CC.
                    common.CC.Disable.Attack != 0
                    // Lost target.
                 || !common.Target.IsTargetExists(selectLookup))
                    common.State.SetIdle();

                else continue;

                IStateExitFunc<AttackState>.MarkExitExecuted(filter);

                // restart attack cooldown if not actually dealt damage yet
                if (!attackData.ValueRO.isAttacked)
                    attackData.ValueRW.ResetCooldown();
            }
        }
    }

    [UpdateInGroup(typeof(StateEnterSystemGroup))]
    public partial struct Enter : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<ClientServerTickRate>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var curTick  = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
            var tickRate = SystemAPI.GetSingleton<ClientServerTickRate>().SimulationTickRate;

            foreach (var (
                    _
                  , anim
                  , attack)
                in SystemAPI.Query<
                    StateFilterAspect
                  , SharedAnimAspect
                  , AttackStateAspectRW>()) {
                anim.SetAnim(SharedAnimKey.Attack);

                attack.RestartAttack(curTick, tickRate);
            }
        }
    }

    [UpdateInGroup(typeof(StateUpdateSystemGroup))]
    public partial struct Update : ISystem {
        [ReadOnly] private ComponentLookup<LocalTransform> locTransLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ClientServerTickRate>();
            state.RequireForUpdate<NetworkTime>();

            locTransLookup = SystemAPI.GetComponentLookup<LocalTransform>(
                isReadOnly: true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            locTransLookup.Update(ref state);

            var curTick  = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
            var tickRate = SystemAPI.GetSingleton<ClientServerTickRate>().SimulationTickRate;

            // DO MELEE ATTACK
            foreach (var (_, attackData, attackTrigger) in SystemAPI
                .Query<
                    StateFilterAspect
                  , RefRW<AttackStateData>
                  , EnabledRefRW<MeleeAttackTrigger>>()
                .WithPresent<MeleeAttackTrigger>())
                if (attackData.ValueRO.IsAttackReady(curTick)) {
                    attackData.ValueRW.MarkAttacked();

                    attackTrigger.ValueRW = true;
                }

            // DO RANGED ATTACK
            foreach (var (_, attackData, attackTrigger) in SystemAPI
                .Query<
                    StateFilterAspect
                  , RefRW<AttackStateData>
                  , EnabledRefRW<RangedAttackTrigger>>()
                .WithPresent<RangedAttackTrigger>())
                if (attackData.ValueRO.IsAttackReady(curTick)) {
                    attackData.ValueRW.MarkAttacked();

                    attackTrigger.ValueRW = true;
                }

            // RESTART ATTACK
            foreach (var (_, anim, attackAspect) in SystemAPI
                .Query<
                    StateFilterAspect
                  , RefRW<SharedAnimData>
                  , AttackStateAspectRW>())
                if (attackAspect.Data.IsCooldownDone(curTick)) {
                    attackAspect.RestartAttack(curTick, tickRate);
                    anim.ValueRW.MarkNeedRestart();
                }

            // ROTATE TO TARGET
            foreach (var (_, rotationData, target, locTrans) in SystemAPI
                .Query<
                    StateFilterAspect
                  , RefRW<RotationData>
                  , AimedTargetAspectRO
                  , RefRO<LocalTransform>>())
                if (locTransLookup.EntityExists(target.Target))
                    rotationData.ValueRW.RotateTo((
                        locTransLookup[target.Target].Position
                      - locTrans.ValueRO.Position
                    ).Quantizate3().xz);
        }
    }
}

public static partial class ChampionStateAttack {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<ChampionTag, AttackState> {
            private readonly RefRO<ChampionTag>              _identity;
            private readonly RefRO<Simulate>                 _simulate;
            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<AttackState>       _curStateEnable;

            RefRO<ChampionTag> IStateAspect<ChampionTag, AttackState>.Identity => _identity;
            RefRO<Simulate> IStateAspect<ChampionTag, AttackState>.   Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitFunc<AttackState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<AttackState> IStateExitFunc<AttackState>.      CurStateEnable    => _curStateEnable;
        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<ChampionTag, AttackState> {
            private readonly RefRO<ChampionTag> _identity;
            private readonly RefRO<AttackState> _curState;
            private readonly RefRO<Simulate>    _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<ChampionTag> IStateAspect<ChampionTag, AttackState>.Identity => _identity;
            RefRO<AttackState> IStateAspect<ChampionTag, AttackState>.CurState => _curState;
            RefRO<Simulate> IStateAspect<ChampionTag, AttackState>.   Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<ChampionTag, AttackState>.StateRequireEnter => _stateRequireEnter;
        }
    }

    public partial struct Update {
        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<ChampionTag, AttackState> {
            private readonly RefRO<ChampionTag> _identity;
            private readonly RefRO<AttackState> _curState;
            private readonly RefRO<Simulate>    _simulate;

            RefRO<ChampionTag> IStateAspect<ChampionTag, AttackState>.Identity => _identity;
            RefRO<AttackState> IStateAspect<ChampionTag, AttackState>.CurState => _curState;
            RefRO<Simulate> IStateAspect<ChampionTag, AttackState>.   Simulate => _simulate;
        }
    }

    public partial struct FixedUpdate {
        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<ChampionTag, AttackState> {
            private readonly RefRO<ChampionTag> _identity;
            private readonly RefRO<AttackState> _curState;
            private readonly RefRO<Simulate>    _simulate;

            RefRO<ChampionTag> IStateAspect<ChampionTag, AttackState>.Identity => _identity;
            RefRO<AttackState> IStateAspect<ChampionTag, AttackState>.CurState => _curState;
            RefRO<Simulate> IStateAspect<ChampionTag, AttackState>.   Simulate => _simulate;
        }
    }
}