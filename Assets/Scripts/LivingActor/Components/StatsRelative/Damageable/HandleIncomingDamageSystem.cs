using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
public partial struct HandleIncomingDamageSystem : ISystem {
    [ReadOnly] private ComponentLookup<ChampionTag>   champLookup;
    [ReadOnly] private ComponentLookup<ActorDetector> detectorLookup;
    private ComponentLookup<AllyBeAttackedData> allyBeAttackedLookup;
    public NativeList<DistanceHit>             castResult;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<
            IncomingDamageBuffer
          , HealthData
          , Simulate>().Build());

        champLookup = SystemAPI.GetComponentLookup<ChampionTag>(
            isReadOnly: true);
        detectorLookup = SystemAPI.GetComponentLookup<ActorDetector>(
            isReadOnly: true);
        allyBeAttackedLookup = SystemAPI.GetComponentLookup<AllyBeAttackedData>(
            isReadOnly: false);
        castResult = new NativeList<DistanceHit>(Allocator.Persistent);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick) return;
        
        champLookup.Update(ref state);
        detectorLookup.Update(ref state);
        allyBeAttackedLookup.Update(ref state);

        // use without parallel
        state.Dependency = new Job {
            collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld,
            champLookup = champLookup,
            detectorLookup = detectorLookup,
            allyBeAttackedLookup = allyBeAttackedLookup,
            castResult = castResult
        }.Schedule(state.Dependency);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {
        castResult.Dispose();
    }

    /// <summary>
    /// Use without prarallel
    /// </summary>
    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public static readonly CollisionFilter filterActorDetector = new() {
            BelongsTo = PhysicsLayerHelper.All,
            CollidesWith = PhysicsLayerHelper.ActorDetector,
        };

        public CollisionWorld collisionWorld;
        [ReadOnly] public ComponentLookup<ChampionTag>   champLookup;
        [ReadOnly] public ComponentLookup<ActorDetector> detectorLookup;

        public ComponentLookup<AllyBeAttackedData> allyBeAttackedLookup;
        public NativeList<DistanceHit>             castResult;

        [BurstCompile]
        public void Execute(
            ref DynamicBuffer<IncomingDamageBuffer> incomingDamage
          , ref HealthData                          healthData
          , in  LocalTransform                      locTrans
          , in  Entity                              entity) {
            float_Q3 totalDamage = 0;
            foreach (var damage in incomingDamage) {
                totalDamage += damage.damage;

                castResult.Clear();
                if (champLookup.HasComponent(damage.source)
                 && champLookup.HasComponent(entity)
                 && collisionWorld.OverlapCapsule(
                        locTrans.Position.WithY(-1e5f),
                        locTrans.Position.WithY(1e5f),
                        50,
                        ref castResult,
                        filterActorDetector))
                    foreach (var result in castResult)
                        if (detectorLookup.TryGetComponent(result.Entity, out var detector))
                            allyBeAttackedLookup.GetRefRW(detector.holder).ValueRW.champByChamp = damage.source;
            }
            incomingDamage.Clear();
        
            healthData.value -= totalDamage;
        }
    }
}