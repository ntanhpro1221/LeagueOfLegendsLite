using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

public static partial class MonsterStateIdle {
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
              , data
                ) in SystemAPI.Query<
                StateFilterAspect
              , CommonExitStateAspect
              , UpdateAspect>()) {
                bool targetExist  = common.Target.IsTargetExists(selectLookup);
                bool attackCDDone = data.AttackData.IsCooldownDone(curTick);
                bool targetOutOfRange = targetExist
                    ? common.Target.IsTargetOutOfRange(locTransLookup, statsLookup)
                    : true;

                // DEAD STATE
                if (common.Health.IsDead) // Run out of health.
                    common.State.SetDead();

                // MOVE STATE
                else if (
                    // Not have disabling move CC.
                    common.CC.Disable.Move == 0 && (
                        // In disabling leash state or
                        data.IsLeashDisabling
                     || ( // In Tracing target
                            // Exist target
                            targetExist
                            // Leashing target
                         && data.IsLeashing
                            // Target is out of range
                         && targetOutOfRange)))
                    common.State.SetMove();

                // ATTACK STATE
                else if (
                    // Not have disabling attack CC.
                    common.CC.Disable.Attack == 0
                    // Exist target
                 && targetExist
                    // Leashing target
                 && data.IsLeashing
                    // Target in range
                 && !targetOutOfRange
                    // Attack cool down done
                 && attackCDDone)
                    common.State.SetAttack();

                else continue;

                filter.MarkExitExecuted();
            }
        }

        private readonly partial struct UpdateAspect : IAspect {
            private readonly RefRO<AttackStateData> _AttackData;

            [Optional] private readonly EnabledRefRO<MonsterLeashAnchor>    _LeashTrigger;
            [Optional] private readonly EnabledRefRO<MonsterLeashDisabling> _UnleashTrigger;

            public ref readonly AttackStateData AttackData => ref _AttackData.ValueRO;

            public bool IsLeashing       => _LeashTrigger.ValueRO;
            public bool IsLeashDisabling => _UnleashTrigger.ValueRO;
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

            // ROTATE TO INIT TRANSFORM
            foreach (var (_, rotationData, anchor) in SystemAPI
                .Query<
                    StateFilterAspect
                  , RefRW<RotationData>
                  , RefRO<MonsterLeashAnchor>>()
                .WithDisabled<
                    MonsterLeashAnchor
                  , MonsterLeashDisabling>())
                rotationData.ValueRW.RotateTo(anchor.ValueRO.anchorDir);
        }
    }
}

public static partial class MonsterStateIdle {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<MonsterTag, IdleState> {
            private readonly RefRO<MonsterTag> _identity;
            private readonly RefRO<Simulate>   _simulate;

            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<IdleState>         _curStateEnable;

            RefRO<MonsterTag> IStateAspect<MonsterTag, IdleState>.Identity => _identity;
            RefRO<Simulate> IStateAspect<MonsterTag, IdleState>.  Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitAspect<MonsterTag, IdleState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<IdleState> IStateExitAspect<MonsterTag, IdleState>.        CurStateEnable    => _curStateEnable;

            public void MarkExitExecuted() => _stateNotExitedYet.ValueRW = _curStateEnable.ValueRW = false;
        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<MonsterTag, IdleState> {
            private readonly RefRO<MonsterTag> _identity;
            private readonly RefRO<IdleState>  _curState;
            private readonly RefRO<Simulate>   _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<MonsterTag> IStateAspect<MonsterTag, IdleState>.Identity => _identity;
            RefRO<IdleState> IStateAspect<MonsterTag, IdleState>. CurState => _curState;
            RefRO<Simulate> IStateAspect<MonsterTag, IdleState>.  Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<MonsterTag, IdleState>.StateRequireEnter => _stateRequireEnter;
        }
    }

    public partial struct Update {
        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<MonsterTag, IdleState> {
            private readonly RefRO<MonsterTag> _identity;
            private readonly RefRO<IdleState>  _curState;
            private readonly RefRO<Simulate>   _simulate;

            RefRO<MonsterTag> IStateAspect<MonsterTag, IdleState>.Identity => _identity;
            RefRO<IdleState> IStateAspect<MonsterTag, IdleState>. CurState => _curState;
            RefRO<Simulate> IStateAspect<MonsterTag, IdleState>.  Simulate => _simulate;
        }
    }
}