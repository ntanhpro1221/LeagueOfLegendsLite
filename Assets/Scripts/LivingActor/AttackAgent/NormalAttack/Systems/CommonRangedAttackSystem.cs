using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(NormalAttackSystemGroup))]
public partial struct CommonRangedAttackSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        using var ecb         = new EntityCommandBuffer(Allocator.TempJob);
        var       ecbParallel = ecb.AsParallelWriter();

        state.Dependency = new Job {
            ecbParallel                    = ecbParallel
          , isFirstTimeFullyPredictingTick = SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick
        }.ScheduleParallel(state.Dependency);

        state.CompleteDependency();

        ecb.Playback(state.EntityManager);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    public partial struct Job : IJobEntity {
        public EntityCommandBuffer.ParallelWriter ecbParallel;

        public bool isFirstTimeFullyPredictingTick;

        [BurstCompile]
        public void Execute(
            in AimedTargetData                                 target
          , in RangedAttackTriggerData                         attackData
          , in LocalTransform                                  locTrans
          , in ProjectileSpawnPoint                            projSpawnPnt
          , in RotationData                                    rotationData
          , in DynamicBuffer<DamageTriggerSource.EffectBuffer> onHitEffects
          , ScalerPersonalConstructAspect                      personalConstructor
          , EnabledRefRW<RangedAttackTrigger>                  attackTrigger
          , in                   Entity                        entity
          , [EntityIndexInQuery] int                           queryId) {
            // Do real spawn once
            if (isFirstTimeFullyPredictingTick) {
                var projectile = ecbParallel.Instantiate(queryId, attackData.projectile);

                // Set target
                ecbParallel.SetComponent(queryId, projectile, new AimedTargetData { target = target.target });

                // Set transform
                ecbParallel.SetComponent(queryId, projectile, LocalTransform.FromPositionRotation(
                    LocalTransform
                        .FromPositionRotation(locTrans.Position, rotationData.quaternion)
                        .TransformPoint(projSpawnPnt.point.position)
                  , rotationData.quaternion));

                // Set damage data
                ecbParallel.SetComponent(queryId, projectile, new DamageTriggerSource {
                    damage       = personalConstructor.Stats.PhysicDamage
                  , source       = entity
                  , sourcePos    = locTrans.Position.Quantizate3()
                  , sourceScaler = personalConstructor.Construct()
                });

                // Set on-hit effect
                foreach (var effect in onHitEffects) ecbParallel.AppendToBuffer(queryId, projectile, effect);
            }

            attackTrigger.ValueRW = false;
        }
    }
}