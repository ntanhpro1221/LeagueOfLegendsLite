using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

public static partial class AsheStateSkill_E {
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
                  , common
                  , commonChamp
                  , stateData)
                in SystemAPI.Query<
                    StateFilterAspect
                  , CommonExitStateAspect
                  , CommonExitStateAspect_Champion
                  , RefRW<ItemCommonStateData>>())
                if (SMHelpers.TryExit<Skill_E_State>.ItemCommon(filter, common, commonChamp, stateData.ValueRO, selectLookup, curTick)) {
                    // Do something when exit here
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
              , RefRW<ItemCommonStateData>
              , RefRO<SharedAnimData>
              , RefRW<RotationData>>()) {
                anim.SetAnim(SharedAnimKey.Skill_E);

                var animTick = animData.ValueRO.AnimLengthTicks[SharedAnimKey.Skill_E];
                stateData.ValueRW.performData.Enter(curTick, animTick, 0.2f);

                rotation.ValueRW.RotateTo(stateData.ValueRO.input.direction);
            }
        }
    }

    [UpdateInGroup(typeof(StateUpdateSystemGroup))]
    public partial struct Update : ISystem {
        public const float ARROW_DISTANCE         = 30000;
        public const float ARROW_START_DEL        = 200;
        public const float ARROW_START_LERP_RATIO = ARROW_START_DEL / ARROW_DISTANCE;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (
                _
              , stateData
              , locTrans
              , prefabBuffer
              , projectileSpawnPoint
              , teamData
                ) in SystemAPI
                .Query<
                    StateFilterAspect
                  , RefRW<ItemCommonStateData>
                  , RefRO<LocalTransform>
                  , DynamicBuffer<AsheSkill_E.PrefabBuffer>
                  , RefRO<ProjectileSpawnPoint>
                  , RefRO<TeamTypeData>>()) {
                var netTime = SystemAPI.GetSingleton<NetworkTime>();
                if (!stateData.ValueRO.performData.IsReadyToPerform(netTime.ServerTick)) continue;
                stateData.ValueRW.performData.MarkPerformed();

                if (!netTime.IsFirstTimeFullyPredictingTick) continue;
                var direction = quaternion.LookRotation(stateData.ValueRO.input.direction.Full, math.up());
                var spawnPoint = LocalTransform.FromPositionRotation(locTrans.ValueRO.Position, direction)
                    .TransformPoint(projectileSpawnPoint.ValueRO.point.position);

                Entity prefab = prefabBuffer[(int)AsheSkill_E.ConcretePrefab.arrow].entity;

                var arrow         = ecb.Instantiate(prefab);
                var arrowRotation = direction;
                var toEndPointVec = ARROW_DISTANCE * math.normalize(arrowRotation.Forward().xz);
                var destination   = spawnPoint.Quantizate3() + new float3_Q3(toEndPointVec.x, 0, toEndPointVec.y);

                ecb.SetComponent(arrow, LocalTransform.FromPositionRotation(math.lerp(spawnPoint, destination, ARROW_START_LERP_RATIO), arrowRotation));

                ecb.SetComponent(arrow, new DestroyAtDestination { destination = destination });

                ecb.SetComponent(arrow, teamData.ValueRO);

                MoveRequesterAspect.MoveStraightTo(ref ecb, arrow, destination);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}

public static partial class AsheStateSkill_E {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<AsheTag, Skill_E_State> {
            private readonly RefRO<AsheTag>                  _identity;
            private readonly RefRO<Simulate>                 _simulate;
            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<Skill_E_State>     _curStateEnable;

            RefRO<AsheTag> IStateAspect<AsheTag, Skill_E_State>. Identity => _identity;
            RefRO<Simulate> IStateAspect<AsheTag, Skill_E_State>.Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitFunc<Skill_E_State>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<Skill_E_State> IStateExitFunc<Skill_E_State>.    CurStateEnable    => _curStateEnable;
        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<AsheTag, Skill_E_State> {
            private readonly RefRO<AsheTag>       _identity;
            private readonly RefRO<Skill_E_State> _curState;
            private readonly RefRO<Simulate>      _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<AsheTag> IStateAspect<AsheTag, Skill_E_State>.      Identity => _identity;
            RefRO<Skill_E_State> IStateAspect<AsheTag, Skill_E_State>.CurState => _curState;
            RefRO<Simulate> IStateAspect<AsheTag, Skill_E_State>.     Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<AsheTag, Skill_E_State>.StateRequireEnter => _stateRequireEnter;
        }
    }

    public partial struct Update {
        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<AsheTag, Skill_E_State> {
            private readonly RefRO<AsheTag>       _identity;
            private readonly RefRO<Skill_E_State> _curState;
            private readonly RefRO<Simulate>      _simulate;

            RefRO<AsheTag> IStateAspect<AsheTag, Skill_E_State>.      Identity => _identity;
            RefRO<Skill_E_State> IStateAspect<AsheTag, Skill_E_State>.CurState => _curState;
            RefRO<Simulate> IStateAspect<AsheTag, Skill_E_State>.     Simulate => _simulate;
        }
    }

    public partial struct FixedUpdate {
        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<AsheTag, Skill_E_State> {
            private readonly RefRO<AsheTag>       _identity;
            private readonly RefRO<Skill_E_State> _curState;
            private readonly RefRO<Simulate>      _simulate;

            RefRO<AsheTag> IStateAspect<AsheTag, Skill_E_State>.      Identity => _identity;
            RefRO<Skill_E_State> IStateAspect<AsheTag, Skill_E_State>.CurState => _curState;
            RefRO<Simulate> IStateAspect<AsheTag, Skill_E_State>.     Simulate => _simulate;
        }
    }
}