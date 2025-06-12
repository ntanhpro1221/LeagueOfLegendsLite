using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

public static partial class AsheStateSKill_W {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    public partial struct Exit : ISystem {
        [ReadOnly] private ComponentLookup<Selectable> selectLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();

            selectLookup = SystemAPI.GetComponentLookup<Selectable>(
                isReadOnly: true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            selectLookup.Update(ref state);

            var curTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            foreach (var (
                    filter
                  , sharedState
                  , health
                  , aimedTarget
                  , stateData
                  , input
                  , itemRequest)
                in SystemAPI.Query<
                    StateFilterAspect
                  , ActorSharedStateAspect
                  , HealthAspectRO
                  , AimedTargetAspectRO
                  , RefRO<CommonItemActiveStateData>
                  , PlayerInputAspectRO
                  , RefRO<ItemActiveNewStateRequestData>>()) {

                // DEAD STATE
                if (health.IsDead) // Run out of health
                    sharedState.SetDead();

                // BLOCK ALL OTHER STATE WHEN HASN'T PERFORMED YET
                else if (!stateData.ValueRO.performData.isPerformed)
                    continue;

                // ITEM ANALYZING STATE
                else if (itemRequest.ValueRO.haveRequest)
                    sharedState.SetItemActiveAnalyzing();

                // MOVE STATE
                else if (input.MoveEvent_WithData) // Have move request
                    sharedState.SetMove();

                // ATTACK STATE
                else if (aimedTarget.IsTargetExists(selectLookup)) // Have target
                    sharedState.SetAttack();

                // IDLE STATE
                else if (curTick.IsNewerThan(stateData.ValueRO.performData.doneTick)) // Completely done skill
                    sharedState.SetIdle();
                else continue;

                filter.MarkExitExecuted();
            }
        }
    }

    [UpdateInGroup(typeof(StateEnterSystemGroup))]
    public partial struct Enter : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var curTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            foreach (var (
                _
              , anim
              , stateData
              , animData
              , rotation
                ) in SystemAPI.Query<
                StateFilterAspect
              , SharedAnimAspect
              , RefRW<CommonItemActiveStateData>
              , RefRO<SharedAnimData>
              , RefRW<RotationData>>()) {
                anim.SetAnim(SharedAnimKey.Skill_W);

                var animTick = animData.ValueRO.AnimLengthTicks[SharedAnimKey.Skill_W];
                stateData.ValueRW.performData.Enter(
                    _performTick: curTick.WithBonusTick((uint)(animTick * 0.2))
                  , _doneTick: curTick.WithBonusTick(animTick));

                rotation.ValueRW.RotateTo(stateData.ValueRO.inputForActive.direction);
            }
        }
    }

    [UpdateInGroup(typeof(StateUpdateSystemGroup))]
    public partial struct Update : ISystem {
        public const float ARROW_ANGLE_DIS_RAD    = 5 * Mathf.Deg2Rad;
        public const float ARROW_DISTANCE         = 1000;
        public const float ARROW_START_DEL        = 30;
        public const float ARROW_START_LERP_RATIO = ARROW_START_DEL / ARROW_DISTANCE;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // DO MELEE ATTACK
            foreach (var (
                _
              , data
              , entity
                ) in SystemAPI
                .Query<
                    StateFilterAspect
                  , UpdateAspect
                >().WithEntityAccess()) {
                var netTime = SystemAPI.GetSingleton<NetworkTime>();
                if (!data.StateData.performData.IsReadyToPerform(netTime.ServerTick)) continue;
                data.StateData.performData.MarkPerformed();

                if (!netTime.IsFirstTimeFullyPredictingTick) continue;
                ref var item  = ref data.ItemData.Static[PlayerTrigger.Item.Skill_W];
                var     level = data.ItemData.Dynamic[(int)PlayerTrigger.Item.Skill_W].level;

                var direction = quaternion.LookRotation(data.StateData.inputForActive.direction.Full, math.up());
                var spawnPoint = LocalTransform.FromPositionRotation(data.Position, direction)
                    .TransformPoint(data.ProjectileSpawnPoint.position);

                int      amount = (int)item.concreteProp.Value[(int)AsheSkill_W.ConcreteProperty.arrowAmount][level];
                float_Q3 damage = item.concreteProp.Value[(int)AsheSkill_W.ConcreteProperty.damage][level];
                Entity   prefab = data.PrefabBuffer[(int)AsheSkill_W.ConcretePrefab.arrow].entity;

                var curDelRad = ARROW_ANGLE_DIS_RAD * ((float)amount - 1) / 2;
                for (int i = 0; i < amount; ++i, curDelRad -= ARROW_ANGLE_DIS_RAD) {
                    var arrow         = ecb.Instantiate(prefab);
                    var arrowRotation = math.mul(quaternion.RotateY(curDelRad), direction);
                    var toEndPointVec = ARROW_DISTANCE * math.normalize(arrowRotation.Forward().xz);
                    var destination   = spawnPoint.Quantizate3() + new float3_Q3(toEndPointVec.x, 0, toEndPointVec.y);

                    ecb.SetComponent(arrow, LocalTransform.FromPositionRotation(math.lerp(spawnPoint, destination, ARROW_START_LERP_RATIO), arrowRotation));

                    ecb.SetComponent(arrow, new DestroyAtDestination { destination = destination });

                    ecb.SetComponent(arrow, new DamageTriggerSource {
                        damage       = damage
                      , source       = entity
                      , sourcePos    = data.Position.Quantizate3()
                      , sourceScaler = data.PersonalConstructor.Construct()
                    });

                    ecb.SetComponent(arrow, data.Team);

                    MoveRequesterAspect.MoveStraightTo(ref ecb, arrow, destination);
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        private readonly partial struct UpdateAspect : IAspect {
            private readonly RefRW<CommonItemActiveStateData> _StateData;
            private readonly RefRO<LocalTransform>            _LocTrans;
            private readonly RefRO<ProjectileSpawnPoint>      _ProjectileSpawnPoint;
            private readonly RefRO<TeamTypeData>              _TeamData;

            [ReadOnly] public readonly DynamicBuffer<AsheSkill_W.PrefabBuffer> PrefabBuffer;

            public readonly ItemDataAspectRO              ItemData;
            public readonly ScalerPersonalConstructAspect PersonalConstructor;

            public ref CommonItemActiveStateData StateData => ref _StateData.ValueRW;

            public ref readonly float3        Position             => ref _LocTrans.ValueRO.Position;
            public ref readonly InitTransform ProjectileSpawnPoint => ref _ProjectileSpawnPoint.ValueRO.point;
            public ref readonly TeamTypeData  Team                 => ref _TeamData.ValueRO;
        }
    }
}

public static partial class AsheStateSKill_W {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<AsheTag, Skill_W_State> {
            private readonly RefRO<AsheTag>                  _identity;
            private readonly RefRO<Simulate>                 _simulate;
            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<Skill_W_State>     _curStateEnable;

            RefRO<AsheTag> IStateAspect<AsheTag, Skill_W_State>. Identity => _identity;
            RefRO<Simulate> IStateAspect<AsheTag, Skill_W_State>.Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitAspect<AsheTag, Skill_W_State>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<Skill_W_State> IStateExitAspect<AsheTag, Skill_W_State>.    CurStateEnable    => _curStateEnable;

            public void MarkExitExecuted() => _stateNotExitedYet.ValueRW = _curStateEnable.ValueRW = false;
        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<AsheTag, Skill_W_State> {
            private readonly RefRO<AsheTag>       _identity;
            private readonly RefRO<Skill_W_State> _curState;
            private readonly RefRO<Simulate>      _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<AsheTag> IStateAspect<AsheTag, Skill_W_State>.      Identity => _identity;
            RefRO<Skill_W_State> IStateAspect<AsheTag, Skill_W_State>.CurState => _curState;
            RefRO<Simulate> IStateAspect<AsheTag, Skill_W_State>.     Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<AsheTag, Skill_W_State>.StateRequireEnter => _stateRequireEnter;
        }
    }

    public partial struct Update {
        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<AsheTag, Skill_W_State> {
            private readonly RefRO<AsheTag>       _identity;
            private readonly RefRO<Skill_W_State> _curState;
            private readonly RefRO<Simulate>      _simulate;

            RefRO<AsheTag> IStateAspect<AsheTag, Skill_W_State>.      Identity => _identity;
            RefRO<Skill_W_State> IStateAspect<AsheTag, Skill_W_State>.CurState => _curState;
            RefRO<Simulate> IStateAspect<AsheTag, Skill_W_State>.     Simulate => _simulate;
        }
    }

    public partial struct FixedUpdate {
        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<AsheTag, Skill_W_State> {
            private readonly RefRO<AsheTag>       _identity;
            private readonly RefRO<Skill_W_State> _curState;
            private readonly RefRO<Simulate>      _simulate;

            RefRO<AsheTag> IStateAspect<AsheTag, Skill_W_State>.      Identity => _identity;
            RefRO<Skill_W_State> IStateAspect<AsheTag, Skill_W_State>.CurState => _curState;
            RefRO<Simulate> IStateAspect<AsheTag, Skill_W_State>.     Simulate => _simulate;
        }
    }
}