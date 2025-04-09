using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

public static partial class ChampionStateAttack {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    public partial struct Exit : ISystem {
        private EntityStorageInfoLookup       entityLookup;
        private ComponentLookup<LocalToWorld> l2wLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EnumIndexData>();

            entityLookup = state.GetEntityStorageInfoLookup();
            l2wLookup    = SystemAPI.GetComponentLookup<LocalToWorld>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            entityLookup.Update(ref state);
            l2wLookup.Update(ref state);

            var attackRangeId = SystemAPI.GetSingleton<EnumIndexData>().StatsType[StatsType.AttackRange];

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

                // MOVE STATE
                else if (aimedTarget.NeedMoveToTarget(entityLookup, attackRangeId, l2wLookup))
                    sharedState.SetMove();
  
                // IDLE STATE
                else if (!aimedTarget.IsTargetExists(entityLookup)) // Lost target
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
                Debug.Log("Attack");
                
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
                    attackAspect.RestartAttack(curTick, attackSpeedId, tickRate);
                    anim.ValueRW.MarkNeedRestart();
                }
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

            EnabledRefRW<StateNotExitedYet> IStateExitAspect<ChampionTag, AttackState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<AttackState> IStateExitAspect<ChampionTag, AttackState>.      CurStateEnable    => _curStateEnable;

            public void MarkExitExecuted() => _stateNotExitedYet.ValueRW = _curStateEnable.ValueRW = false;
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