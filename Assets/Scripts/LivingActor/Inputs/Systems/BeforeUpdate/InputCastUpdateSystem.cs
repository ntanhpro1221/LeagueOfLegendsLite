using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;

[UpdateInGroup(typeof(BeforeInputLocalUpdateSystemGroup))]
[UpdateAfter(typeof(InputDirtyUpdateSystem))]
public partial struct InputCastUpdateSystem : ISystem {
    /// <summary>
    /// Because max fraction is 1
    /// </summary>
    private const float MAX_NULL_HIT_FRACTION = 2;

    private const uint GROUND_ACTOR =
        PhysicsLayerHelper.Ground
      | PhysicsLayerHelper.Actor;

    private static readonly CollisionFilter filterGroundActor = new() {
        BelongsTo    = PhysicsLayerHelper.All
      , CollidesWith = GROUND_ACTOR
    };

    private EntityQuery                 ownChampQuery;
    private NativeList<RaycastHit>      castGroundActorResult;
    private ComponentLookup<Selectable> selectLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<InputCastData>();
        state.RequireForUpdate<InputDirtyData>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate(ownChampQuery = SystemAPI.QueryBuilder()
            .WithAll<
                ChampionTag
              , GhostOwnerIsLocal
              , TeamTypeData
            >().WithNone<
                DummyTag
            >().Build());

        castGroundActorResult = new NativeList<RaycastHit>(Allocator.Persistent);
        selectLookup = SystemAPI.GetComponentLookup<Selectable>(
            isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.CompleteDependency();
        
        ref var castData = ref SystemAPI.GetSingletonRW<InputCastData>().ValueRW;
        castData.Reset();

        var dirtyData        = SystemAPI.GetSingleton<InputDirtyData>();

        if (dirtyData.isPointerOverUI) return;
        
        var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        castGroundActorResult.Clear();
        if (!collisionWorld.CastRay(new RaycastInput {
            Start  = dirtyData.mouse_ray_start
          , End    = dirtyData.mouse_ray_end
          , Filter = filterGroundActor
        }, ref castGroundActorResult)) return;

        selectLookup.Update(ref state);

        var bodies = collisionWorld.Bodies;
        float actorFraction  = MAX_NULL_HIT_FRACTION
            , groundFraction = MAX_NULL_HIT_FRACTION;
        foreach (var actorGroundHit in castGroundActorResult)
            switch (bodies[actorGroundHit.RigidBodyIndex].Collider.Value
                .GetCollisionFilter(actorGroundHit.ColliderKey).BelongsTo) {

                case PhysicsLayerHelper.Actor:
                    if (!selectLookup.HasComponent(actorGroundHit.Entity) || !selectLookup.IsComponentEnabled(actorGroundHit.Entity))
                        break;

                    if (actorFraction < actorGroundHit.Fraction)
                        break;
                    actorFraction = actorGroundHit.Fraction;

                    if (SystemAPI.GetComponent<TeamTypeData>(actorGroundHit.Entity).team
                     == ownChampQuery.GetSingleton<TeamTypeData>().team)
                        castData.SetHitAlly(actorGroundHit.Entity);
                    else castData.SetHitEnemy(actorGroundHit.Entity);

                    break;

                case PhysicsLayerHelper.Ground:
                    if (groundFraction < actorGroundHit.Fraction)
                        break;
                    groundFraction = actorGroundHit.Fraction;

                    castData.SetHitGroundAt(actorGroundHit.Position.Quantizate3());

                    break;
            }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {
        castGroundActorResult.Dispose();
    }
}