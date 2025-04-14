using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

public static partial class TurretStateAttack {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    public partial struct Exit : ISystem {
        [ReadOnly] private EntityStorageInfoLookup         entityLookup;
        [ReadOnly] private ComponentLookup<Selectable>     selectLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EnumIndexData>();

            entityLookup = state.GetEntityStorageInfoLookup();
            selectLookup = SystemAPI.GetComponentLookup<Selectable>(
                isReadOnly: true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            entityLookup.Update(ref state);
            selectLookup.Update(ref state);

            ref var statsId       = ref SystemAPI.GetSingleton<EnumIndexData>().StatsType;
            var     attackRangeId = statsId[StatsType.AttackRange];
            var     unitRadiusId  = statsId[StatsType.UnitRadius];
            
            foreach (var (
                    filter
                  , sharedState
                  , health
                  , aimedTarget
                  , attackData)
                in SystemAPI.Query<
                    StateFilterAspect
                  , ActorSharedStateAspect
                  , HealthAspectRO
                  , AimedTargetAspectRO
                  , RefRW<AttackStateData>>()) {

                // DEAD STATE
                if (health.IsDead) // Run out of health
                    sharedState.SetDead();

                // IDLE STATE
                else if (!aimedTarget.IsTargetExists(entityLookup, selectLookup)) // Lost target
                    sharedState.SetIdle();
                else continue;

                filter.MarkExitExecuted();

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
            state.RequireForUpdate<EnumIndexData>();
            state.RequireForUpdate<ClientServerTickRate>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var curTick       = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
            var attackSpeedId = SystemAPI.GetSingleton<EnumIndexData>().StatsType[StatsType.AttackSpeed];
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

                attack.RestartAttack(curTick, attackSpeedId, tickRate);
            }
        }
    }

    [UpdateInGroup(typeof(StateUpdateSystemGroup))]
    public partial struct Update : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ClientServerTickRate>();
            state.RequireForUpdate<EnumIndexData>();
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var curTick       = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
            var attackSpeedId = SystemAPI.GetSingleton<EnumIndexData>().StatsType[StatsType.AttackSpeed];
            var tickRate      = SystemAPI.GetSingleton<ClientServerTickRate>().SimulationTickRate;

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
                    attackAspect.RestartAttack(curTick, attackSpeedId, tickRate);
                    anim.ValueRW.MarkNeedRestart();
                }
        }
    }
}

public static partial class TurretStateAttack {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<TurretTag, AttackState> {
            private readonly RefRO<TurretTag>              _identity;
            private readonly RefRO<Simulate>                 _simulate;
            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<AttackState>       _curStateEnable;

            RefRO<TurretTag> IStateAspect<TurretTag, AttackState>.Identity => _identity;
            RefRO<Simulate> IStateAspect<TurretTag, AttackState>.   Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitAspect<TurretTag, AttackState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<AttackState> IStateExitAspect<TurretTag, AttackState>.      CurStateEnable    => _curStateEnable;

            public void MarkExitExecuted() => _stateNotExitedYet.ValueRW = _curStateEnable.ValueRW = false;
        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<TurretTag, AttackState> {
            private readonly RefRO<TurretTag> _identity;
            private readonly RefRO<AttackState> _curState;
            private readonly RefRO<Simulate>    _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<TurretTag> IStateAspect<TurretTag, AttackState>.Identity => _identity;
            RefRO<AttackState> IStateAspect<TurretTag, AttackState>.CurState => _curState;
            RefRO<Simulate> IStateAspect<TurretTag, AttackState>.   Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<TurretTag, AttackState>.StateRequireEnter => _stateRequireEnter;
        }
    }

    public partial struct Update {
        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<TurretTag, AttackState> {
            private readonly RefRO<TurretTag> _identity;
            private readonly RefRO<AttackState> _curState;
            private readonly RefRO<Simulate>    _simulate;

            RefRO<TurretTag> IStateAspect<TurretTag, AttackState>.Identity => _identity;
            RefRO<AttackState> IStateAspect<TurretTag, AttackState>.CurState => _curState;
            RefRO<Simulate> IStateAspect<TurretTag, AttackState>.   Simulate => _simulate;
        }
    }

    public partial struct FixedUpdate {
        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<TurretTag, AttackState> {
            private readonly RefRO<TurretTag> _identity;
            private readonly RefRO<AttackState> _curState;
            private readonly RefRO<Simulate>    _simulate;

            RefRO<TurretTag> IStateAspect<TurretTag, AttackState>.Identity => _identity;
            RefRO<AttackState> IStateAspect<TurretTag, AttackState>.CurState => _curState;
            RefRO<Simulate> IStateAspect<TurretTag, AttackState>.   Simulate => _simulate;
        }
    }
}