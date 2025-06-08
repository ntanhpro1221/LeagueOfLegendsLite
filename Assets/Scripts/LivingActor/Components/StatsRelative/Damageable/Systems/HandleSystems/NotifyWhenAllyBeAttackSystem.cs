using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(HandleIncomingDamageSystemGroup))]
public partial struct NotifyWhenAllyBeAttackSystem : ISystem {
    [ReadOnly] private ComponentLookup<ChampionTag>        champLookup;
    [ReadOnly] private ComponentLookup<ActorDetector>      detectorLookup;
    private            ComponentLookup<AllyBeAttackedData> allyBeAttackedLookup;
    private            NativeList<DistanceHit>             castResult;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<PhysicsWorldSingleton>();

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
        champLookup.Update(ref state);
        detectorLookup.Update(ref state);
        allyBeAttackedLookup.Update(ref state);

        // use without parallel
        state.Dependency = new Job {
            collisionWorld       = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld
          , champLookup          = champLookup
          , detectorLookup       = detectorLookup
          , allyBeAttackedLookup = allyBeAttackedLookup
          , castResult           = castResult
        }.Schedule(state.Dependency);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {
        castResult.Dispose();
    }

    /// <summary>
    /// Use without parallel
    /// </summary>
    [WithAll(
        typeof(Simulate)
      , typeof(HealthData))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        private static readonly CollisionFilter filterActorDetector = new() {
            BelongsTo    = PhysicsLayerHelper.All
          , CollidesWith = PhysicsLayerHelper.ActorDetector,
        };

        public CollisionWorld collisionWorld;

        [ReadOnly] public ComponentLookup<ChampionTag>   champLookup;
        [ReadOnly] public ComponentLookup<ActorDetector> detectorLookup;

        public ComponentLookup<AllyBeAttackedData> allyBeAttackedLookup;
        public NativeList<DistanceHit>             castResult;

        [BurstCompile]
        public void Execute(
            in DynamicBuffer<IncomingDamageBuffer> incomingDamage
          , in LocalTransform                      locTrans
          , in Entity                              entity) {
            foreach (var damage in incomingDamage) {
                castResult.Clear();
                if ( // Both sender and receiver must are champions
                    champLookup.HasComponent(damage.source) && champLookup.HasComponent(entity)
                 && collisionWorld.OverlapCapsule(
                        locTrans.Position.WithY(-1e5f),
                        locTrans.Position.WithY(1e5f),
                        65,
                        ref castResult,
                        filterActorDetector))
                    foreach (var result in castResult)
                        if (detectorLookup.TryGetComponent(result.Entity, out var detector)
                         && allyBeAttackedLookup.HasComponent(detector.holder))
                            allyBeAttackedLookup.GetRefRW(detector.holder).ValueRW.champByChamp = damage.source;
            }
        }
    }
}