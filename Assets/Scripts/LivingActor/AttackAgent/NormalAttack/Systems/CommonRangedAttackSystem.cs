using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(NormalAttackSystemGroup))]
public partial struct CommonRangedAttackSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<EnumIndexData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        state.Dependency = new Job {
            ecb                            = ecb
          , damageId                       = SystemAPI.GetSingleton<EnumIndexData>().StatsType[StatsType.PhysicDamage]
          , isFirstTimeFullyPredictingTick = SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick
        }.Schedule(state.Dependency);

        state.CompleteDependency();

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    public partial struct Job : IJobEntity {
        public EntityCommandBuffer ecb;
        public int                 damageId;
        public bool                isFirstTimeFullyPredictingTick;

        [BurstCompile]
        public void Execute(
            in AimedTargetData                target
          , in RangedAttackTriggerData        attackData
          , in LocalTransform                 locTrans
          , in ProjectileSpawnPoint           projSpawnPnt
          , in DynamicBuffer<StatsBuffer>     stats
          , EnabledRefRW<RangedAttackTrigger> attackTrigger
          , in Entity entity) {
            if (isFirstTimeFullyPredictingTick) {
                var projectile = ecb.Instantiate(attackData.projectile);

                ecb.SetComponent(projectile, new AimedTargetData { target = target.target });
                ecb.SetComponent(projectile, new DamageTriggerSource(stats[damageId].value, entity));
                ecb.SetComponent(projectile, LocalTransform.FromPositionRotationScale(
                    // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
                    locTrans.TransformPoint(projSpawnPnt.point.position)
                  , quaternion.identity
                  , 35));
            }

            attackTrigger.ValueRW = false;
        }
    }
}