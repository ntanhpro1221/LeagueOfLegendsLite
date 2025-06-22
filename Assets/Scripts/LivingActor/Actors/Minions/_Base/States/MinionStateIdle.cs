using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

public static partial class MinionStateIdle {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
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
              , attackData
              , fixedPath
                ) in SystemAPI.Query<
                StateFilterAspect
              , CommonExitStateAspect
              , RefRO<AttackStateData>
              , DynamicBuffer<MinionFixedPathBuffer>>()) {
                bool targetExist  = common.Target.IsTargetExists(selectLookup);
                bool attackCDDone = attackData.ValueRO.IsCooldownDone(curTick);

                // DEAD STATE
                if (common.Health.IsDead) // Run out of health.
                    common.State.SetDead();

                // MOVE STATE
                else if (
                    // Not have disabling move CC.
                    common.CC.Disable.Move == 0 && (
                        // Target NOT exist.
                        !targetExist
                        // Or target is out of range.
                     || common.Target.IsTargetOutOfRange(locTransLookup, statsLookup)
                        // Still have path.
                     && !fixedPath.IsEmpty))
                    common.State.SetMove();

                // ATTACK STATE
                else if (
                    // Not have disabling attack CC.
                    common.CC.Disable.Attack == 0
                    // Have target.
                 && targetExist
                    // Attack cool down done.
                 && attackCDDone)
                    common.State.SetAttack();

                else continue;

                IStateExitFunc<IdleState>.MarkExitExecuted(filter);
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

            EnabledRefRW<StateNotExitedYet> IStateExitFunc<IdleState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<IdleState> IStateExitFunc<IdleState>.        CurStateEnable    => _curStateEnable;
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