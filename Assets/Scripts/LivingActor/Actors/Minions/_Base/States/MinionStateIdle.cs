using NGDtuanh.Entities.StateMachine;
using Pathfinding.ECS;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

public static partial class MinionStateIdle {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct Exit : ISystem {
        [ReadOnly] private ComponentLookup<Selectable>     selectLookup;
        [ReadOnly] private ComponentLookup<LocalTransform> locTransLookup;
        [ReadOnly] private BufferLookup<StatsBuffer>       statsLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();

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

            foreach (var (
                    filter
                  , health
                  , aimedTarget
                  , sharedState
                  , attackData)
                in SystemAPI.Query<
                    StateFilterAspect
                  , HealthAspectRO
                  , AimedTargetAspectRO
                  , ActorSharedStateAspect
                  , RefRO<AttackStateData>>()) {
                bool targetExist  = aimedTarget.IsTargetExists(selectLookup);
                bool attackCDDone = attackData.ValueRO.IsCooldownDone(curTick);

                // DEAD STATE
                if (health.IsDead) // RUN OUT OF HEALTH
                    sharedState.SetDead();

                // MOVE STATE
                else if (
                    // Target NOT exist
                    !targetExist
                    // Or target is out of range
                 || aimedTarget.IsTargetOutOfRange(locTransLookup, statsLookup))
                    sharedState.SetMove();

                // ATTACK STATE
                else if (
                    // have target
                    targetExist
                    // attack cool down done
                 && attackCDDone)
                    sharedState.SetAttack();

                else continue;

                filter.MarkExitExecuted();
            }
        }
    }

    [UpdateInGroup(typeof(StateEnterSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
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
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
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

public static partial class MinionStateIdle {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<MinionTag, IdleState> {
            private readonly RefRO<MinionTag> _identity;
            private readonly RefRO<Simulate>  _simulate;

            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<IdleState>         _curStateEnable;

            RefRO<MinionTag> IStateAspect<MinionTag, IdleState>.Identity => _identity;
            RefRO<Simulate> IStateAspect<MinionTag, IdleState>. Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitAspect<MinionTag, IdleState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<IdleState> IStateExitAspect<MinionTag, IdleState>.        CurStateEnable    => _curStateEnable;

            public void MarkExitExecuted() => _stateNotExitedYet.ValueRW = _curStateEnable.ValueRW = false;
        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<MinionTag, IdleState> {
            private readonly RefRO<MinionTag> _identity;
            private readonly RefRO<IdleState> _curState;
            private readonly RefRO<Simulate>  _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<MinionTag> IStateAspect<MinionTag, IdleState>.Identity => _identity;
            RefRO<IdleState> IStateAspect<MinionTag, IdleState>.CurState => _curState;
            RefRO<Simulate> IStateAspect<MinionTag, IdleState>. Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<MinionTag, IdleState>.StateRequireEnter => _stateRequireEnter;
        }
    }

    public partial struct Update {
        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<MinionTag, IdleState> {
            private readonly RefRO<MinionTag> _identity;
            private readonly RefRO<IdleState> _curState;
            private readonly RefRO<Simulate>  _simulate;

            RefRO<MinionTag> IStateAspect<MinionTag, IdleState>.Identity => _identity;
            RefRO<IdleState> IStateAspect<MinionTag, IdleState>.CurState => _curState;
            RefRO<Simulate> IStateAspect<MinionTag, IdleState>. Simulate => _simulate;
        }
    }
}