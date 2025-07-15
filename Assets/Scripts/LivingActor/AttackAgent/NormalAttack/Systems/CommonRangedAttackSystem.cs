using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(NormalAttackSystemGroup))]
public partial struct CommonRangedAttackSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndPredictedSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<NetworkTime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job {
            ecb = SystemAPI
                .GetSingleton<EndPredictedSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged)
                .AsParallelWriter()
          , isFirstTimeFullyPredictingTick = SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public EntityCommandBuffer.ParallelWriter ecb;

        public bool isFirstTimeFullyPredictingTick;

        [BurstCompile]
        private void Execute(
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

            attackTrigger.ValueRW = false;

            // Do real spawn once
            if (!isFirstTimeFullyPredictingTick) return;
            var projectile = ecb.Instantiate(queryId, attackData.projectile);

            // Set target
            ecb.SetComponent(queryId, projectile, target);

            // Set transform
            ecb.SetComponent(queryId, projectile, LocalTransform.FromPositionRotation(
                LocalTransform
                    .FromPositionRotation(locTrans.Position, rotationData.quaternion)
                    .TransformPoint(projSpawnPnt.point.position)
              , rotationData.quaternion));

            // Set damage data
            ecb.SetComponent(queryId, projectile, new DamageTriggerSource {
                damage       = personalConstructor.Stats.PhysicDamage
              , source       = entity
              , sourcePos    = locTrans.Position.Quantizate3()
              , sourceScaler = personalConstructor.Construct()
            });

            // Set on-hit effect
            foreach (var effect in onHitEffects) ecb.AppendToBuffer(queryId, projectile, effect);
        }
    }
}