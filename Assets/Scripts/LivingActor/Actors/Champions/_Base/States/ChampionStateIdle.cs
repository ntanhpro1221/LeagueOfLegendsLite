using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

public static partial class ChampionStateIdle {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    public partial struct Exit : ISystem {
        [ReadOnly] private ComponentLookup<Selectable>     selectLookup;
        [ReadOnly] private ComponentLookup<LocalTransform> locTransLookup;
        [ReadOnly] private BufferLookup<StatsBuffer>       statsLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<EnumIndexData>();

            selectLookup = SystemAPI.GetComponentLookup<Selectable>(
                isReadOnly: true);
            locTransLookup = SystemAPI.GetComponentLookup<LocalTransform>(
                isReadOnly: true);
            statsLookup = SystemAPI.GetBufferLookup<StatsBuffer>(
                isReadOnly: true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            selectLookup.Update(ref state);
            locTransLookup.Update(ref state);
            statsLookup.Update(ref state);

            var curTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            ref var statsId       = ref SystemAPI.GetSingleton<EnumIndexData>().StatsType;
            var     attackRangeId = statsId[StatsType.AttackRange];
            var     unitRadiusId  = statsId[StatsType.UnitRadius];

            foreach (var (
                    filter
                  , health
                  , input
                  , aimedTarget
                  , sharedState
                  , attackData)
                in SystemAPI.Query<
                    StateFilterAspect
                  , HealthAspectRO
                  , RefRO<PlayerInputData>
                  , AimedTargetAspectRO
                  , ActorSharedStateAspect
                  , RefRO<AttackStateData>>()) {

                // DEAD STATE
                if (health.IsDead) // RUN OUT OF HEALTH
                    sharedState.SetDead();

                // MOVE STATE
                else if (
                    // Have move request
                    input.ValueRO.moveEvent.IsSet
                    // Need move to target
                 || aimedTarget.NeedMoveToTarget(selectLookup, attackRangeId, unitRadiusId, locTransLookup, statsLookup)) // HAVE VELOCITY
                    sharedState.SetMove();

                // ATTACK STATE
                else if (
                    // have target
                    aimedTarget.IsTargetExists(selectLookup)
                    // attack cool down done
                 && attackData.ValueRO.IsCooldownDone(curTick))
                    sharedState.SetAttack();
                else continue;

                filter.MarkExitExecuted();
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
            foreach (var (_, moveData, target, locTrans) in SystemAPI
                .Query<
                    StateFilterAspect
                  , RefRW<MoveData>
                  , AimedTargetAspectRO
                  , RefRO<LocalTransform>>())
                if (target.IsTargetExists(selectLookup))
                    moveData.ValueRW.RotateTo(locTrans.ValueRO.Position, target.Target, locTransLookup);
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

            EnabledRefRW<StateNotExitedYet> IStateExitAspect<ChampionTag, IdleState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<IdleState> IStateExitAspect<ChampionTag, IdleState>.        CurStateEnable    => _curStateEnable;

            public void MarkExitExecuted() => _stateNotExitedYet.ValueRW = _curStateEnable.ValueRW = false;
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