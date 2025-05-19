using NGDtuanh.Entities.StateMachine;
using Pathfinding.ECS;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

public static partial class MonsterStateIdle {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
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
                  , data)
                in SystemAPI.Query<
                    StateFilterAspect
                  , UpdateAspect>()) {
                bool targetExist  = data.AimedTarget.IsTargetExists(selectLookup);
                bool attackCDDone = data.AttackData.IsCooldownDone(curTick);
                bool targetOutOfRange = targetExist
                    ? data.AimedTarget.IsTargetOutOfRange(attackRangeId, unitRadiusId, locTransLookup, statsLookup)
                    : true;

                // DEAD STATE
                if (data.Health.IsDead) // RUN OUT OF HEALTH
                    data.SharedState.SetDead();

                // MOVE STATE
                else if (
                    // In disabling leash state or
                    data.IsLeashDisabling
                 || ( // In Tracing target
                        // Exist target
                        targetExist
                        // Leashing target
                     && data.IsLeashing
                        // Target is out of range
                     && targetOutOfRange))
                    data.SharedState.SetMove();

                // ATTACK STATE
                else if (
                    // Exist target
                    targetExist
                    // Leashing target
                 && data.IsLeashing
                    // Target in range
                 && !targetOutOfRange
                    // Attack cool down done
                 && attackCDDone)
                    data.SharedState.SetAttack();

                else continue;

                filter.MarkExitExecuted();

                data.ResetFaceDirection();
            }
        }

        private readonly partial struct UpdateAspect : IAspect {
            public readonly HealthAspectRO         Health;
            public readonly ActorSharedStateAspect SharedState;
            public readonly AimedTargetAspectRO    AimedTarget;

            private readonly RefRW<DestinationPoint> _DesSetter;
            private readonly RefRO<AttackStateData>  _AttackData;

            [Optional] private readonly EnabledRefRO<MonsterLeashAnchor>    _LeashTrigger;
            [Optional] private readonly EnabledRefRO<MonsterLeashDisabling> _UnleashTrigger;

            public ref readonly AttackStateData AttackData => ref _AttackData.ValueRO;

            public bool IsLeashing       => _LeashTrigger.ValueRO;
            public bool IsLeashDisabling => _UnleashTrigger.ValueRO;

            public void ResetFaceDirection() => _DesSetter.ValueRW.facingDirection = default;
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
            foreach (var (
                _
              , desSetter
              , target
              , locTrans) in SystemAPI
                .Query<
                    StateFilterAspect
                  , RefRW<DestinationPoint>
                  , AimedTargetAspectRO
                  , RefRO<LocalTransform>>())
                if (target.IsTargetExists(selectLookup))
                    desSetter.ValueRW.facingDirection = (
                            locTransLookup[target.Target].Position
                          - locTrans.ValueRO.Position)
                        .WithoutY();

            // ROTATE TO INIT TRANSFORM
            foreach (var (
                _
              , desSetter
              , anchor) in SystemAPI
                .Query<
                    StateFilterAspect
                  , RefRW<DestinationPoint>
                  , RefRO<MonsterLeashAnchor>>()
                .WithDisabled<
                    MonsterLeashAnchor
                  , MonsterLeashDisabling>())
                desSetter.ValueRW.facingDirection = anchor.ValueRO.anchorDir.Full;
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