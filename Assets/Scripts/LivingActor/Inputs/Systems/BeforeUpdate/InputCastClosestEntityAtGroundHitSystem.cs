using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

/// <summary>
/// Not run in <see cref="InputCastUpdateSystem"/> because of burst
/// </summary>
[UpdateInGroup(typeof(BeforeInputLocalUpdateSystemGroup))]
[UpdateAfter(typeof(InputCastNearestWalkableGroundSystem))]
public partial struct InputCastClosestEntityAtGroundHitSystem : ISystem {
    public const float MAX_CAST_RADIUS = 160;

    private static readonly CollisionFilter filterActor = LayerId.Actor.ToFilter();

    private NativeList<DistanceHit> castActorResult;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<InputDirtyData>();

        castActorResult = new NativeList<DistanceHit>(Allocator.Persistent);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        ref var castData = ref SystemAPI.GetSingletonRW<InputCastData>().ValueRW;
        if (!castData.isHitWalkableGround) return;

        var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

        castActorResult.Clear();
        if (collisionWorld.OverlapCapsule(
            castData.walkableGroundPos.WithY(-1e5f)
          , castData.walkableGroundPos.WithY(1e5f)
          , MAX_CAST_RADIUS
          , ref castActorResult
          , filterActor)) {
            float curClosest = 1e9f;
            foreach (var actorHit in castActorResult)
                if (actorHit.Distance < curClosest) {
                    curClosest = actorHit.Distance;
                    castData.SetClosestEntityAtGroundHit(actorHit.Entity);
                }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {
        castActorResult.Dispose();
    }
}