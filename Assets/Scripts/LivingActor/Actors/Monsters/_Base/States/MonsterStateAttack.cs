using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

public static partial class MonsterStateAttack {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
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
              , data
                ) in SystemAPI.Query<
                StateFilterAspect
              , CommonExitStateAspect
              , UpdateAspect>()) {
            
                // DEAD STATE
                if (common.Health.IsDead) // Run out of health.
                    common.State.SetDead();
            
                // MOVE STATE
                else if (
                    // Not have disabling move CC.
                    common.CC.Disable.Move == 0 && (
                        // Target no longer exists or
                        !common.Target.IsTargetExists(selectLookup)
                        // In disable leash state or
                     || data.IsLeashDisabling
                     || ( // In tracing target but
                            // already perform attack
                            data.AttackData.isAttacked
                            // and target is out of range now
                         && common.Target.IsTargetOutOfRange(locTransLookup, statsLookup))))
                    common.State.SetMove();
            
                // IDLE STATE
                else if (
                    // Have disabling attack CC.
                    common.CC.Disable.Attack != 0)
                    common.State.SetIdle();
            
                else continue;
            
                IStateExitFunc<AttackState>.MarkExitExecuted(filter);
            
                // restart attack cooldown if not actually dealt damage yet
                if (!data.AttackData.isAttacked)
                    data.AttackData.ResetCooldown();
            }
        }  

        private readonly partial struct UpdateAspect : IAspect {
            private readonly RefRW<AttackStateData> _AttackData;

            [Optional] private readonly EnabledRefRO<MonsterLeashDisabling> _UnleashTrigger;

            public ref AttackStateData AttackData => ref _AttackData.ValueRW;

            public bool IsLeashDisabling => _UnleashTrigger.ValueRO;
        }
    }

    [UpdateInGroup(typeof(StateEnterSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct Enter : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<ClientServerTickRate>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var curTick       = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
            var tickRate      = SystemAPI.GetSingleton<ClientServerTickRate>().SimulationTickRate;

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
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
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

public static partial class MonsterStateAttack {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<MonsterTag, AttackState> {
            private readonly RefRO<MonsterTag>               _identity;
            private readonly RefRO<Simulate>                 _simulate;
            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<AttackState>       _curStateEnable;

            RefRO<MonsterTag> IStateAspect<MonsterTag, AttackState>.Identity => _identity;
            RefRO<Simulate> IStateAspect<MonsterTag, AttackState>.  Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitFunc<AttackState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<AttackState> IStateExitFunc<AttackState>.      CurStateEnable    => _curStateEnable;
        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<MonsterTag, AttackState> {
            private readonly RefRO<MonsterTag>  _identity;
            private readonly RefRO<AttackState> _curState;
            private readonly RefRO<Simulate>    _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<MonsterTag> IStateAspect<MonsterTag, AttackState>. Identity => _identity;
            RefRO<AttackState> IStateAspect<MonsterTag, AttackState>.CurState => _curState;
            RefRO<Simulate> IStateAspect<MonsterTag, AttackState>.   Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<MonsterTag, AttackState>.StateRequireEnter => _stateRequireEnter;
        }
    }

    public partial struct Update {
        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<MonsterTag, AttackState> {
            private readonly RefRO<MonsterTag>  _identity;
            private readonly RefRO<AttackState> _curState;
            private readonly RefRO<Simulate>    _simulate;

            RefRO<MonsterTag> IStateAspect<MonsterTag, AttackState>. Identity => _identity;
            RefRO<AttackState> IStateAspect<MonsterTag, AttackState>.CurState => _curState;
            RefRO<Simulate> IStateAspect<MonsterTag, AttackState>.   Simulate => _simulate;
        }
    }

    public partial struct FixedUpdate {
        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<MonsterTag, AttackState> {
            private readonly RefRO<MonsterTag>  _identity;
            private readonly RefRO<AttackState> _curState;
            private readonly RefRO<Simulate>    _simulate;

            RefRO<MonsterTag> IStateAspect<MonsterTag, AttackState>. Identity => _identity;
            RefRO<AttackState> IStateAspect<MonsterTag, AttackState>.CurState => _curState;
            RefRO<Simulate> IStateAspect<MonsterTag, AttackState>.   Simulate => _simulate;
        }
    }
}