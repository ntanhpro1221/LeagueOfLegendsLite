using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;

[UpdateInGroup(typeof(BeforeInputLocalUpdateSystemGroup))]
[UpdateAfter(typeof(InputDirtyUpdateSystem))]
public partial struct InputCastUpdateSystem : ISystem {
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
        state.RequireForUpdate<InputDirtyData>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate(ownChampQuery = SystemAPI.QueryBuilder()
            .WithAll<
                ChampionTag
              , GhostOwnerIsLocal
              , TeamTypeData>()
            .Build());

        state.EntityManager.CreateSingleton<InputCastData>();
        castGroundActorResult = new NativeList<RaycastHit>(Allocator.Persistent);
        selectLookup = SystemAPI.GetComponentLookup<Selectable>(
            isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.CompleteDependency();
        
        ref var castData = ref SystemAPI.GetSingletonRW<InputCastData>().ValueRW;
        castData.Reset();

        var rayData        = SystemAPI.GetSingleton<InputDirtyData>();
        var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        castGroundActorResult.Clear();
        if (!collisionWorld.CastRay(new RaycastInput {
            Start  = rayData.rayStart
          , End    = rayData.rayEnd
          , Filter = filterGroundActor
        }, ref castGroundActorResult)) return;

        selectLookup.Update(ref state);
        
        uint totalHitLayer = 0;
        uint hitLayer;
        foreach (var actorGroundHit in castGroundActorResult) {
            switch (hitLayer = collisionWorld
                .Bodies[actorGroundHit.RigidBodyIndex]
                .Collider.Value
                .GetCollisionFilter(actorGroundHit.ColliderKey)
                .BelongsTo) {

                case PhysicsLayerHelper.Actor:
                    if (!selectLookup.HasComponent(actorGroundHit.Entity) || !selectLookup.IsComponentEnabled(actorGroundHit.Entity))
                        continue;

                    if (SystemAPI.GetComponent<TeamTypeData>(actorGroundHit.Entity).teamType
                     == ownChampQuery.GetSingleton<TeamTypeData>().teamType)
                        castData.SetHitAlly(actorGroundHit.Entity);
                    else castData.SetHitEnemy(actorGroundHit.Entity);

                    break;

                case PhysicsLayerHelper.Ground:
                    castData.SetHitGroundAt(actorGroundHit.Position.Quantizate3());

                    break;
            }

            if ((totalHitLayer |= hitLayer) == GROUND_ACTOR) 
                break;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {
        castGroundActorResult.Dispose();
    }
}