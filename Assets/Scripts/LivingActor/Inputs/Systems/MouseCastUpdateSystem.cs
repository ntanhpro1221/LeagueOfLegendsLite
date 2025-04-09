using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;

[UpdateInGroup(typeof(BeforePlayerInputUpdateSystemGroup))]
[UpdateAfter(typeof(InputDirtyUpdateSystem))]
public partial struct MouseCastUpdateSystem : ISystem {
    private const uint GROUND_ACTOR = 
        PhysicsLayerHelper.Ground 
      | PhysicsLayerHelper.Actor;

    private static readonly CollisionFilter filterGroundActor = new() {
        BelongsTo    = PhysicsLayerHelper.All
      , CollidesWith = GROUND_ACTOR
    };

    private EntityQuery            ownChampQuery;
    private NativeList<RaycastHit> castResult;

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
        castResult = new NativeList<RaycastHit>(Allocator.Persistent);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        ref var castData = ref SystemAPI.GetSingletonRW<InputCastData>().ValueRW;
        castData.Reset();

        var rayData        = SystemAPI.GetSingleton<InputDirtyData>();
        var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        castResult.Clear();
        if (!collisionWorld.CastRay(new RaycastInput {
            Start  = rayData.rayStart
          , End    = rayData.rayEnd
          , Filter = filterGroundActor
        }, ref castResult)) return;

        uint totalHitLayer = 0;
        uint hitLayer;
        foreach (var hit in castResult) {
            switch (hitLayer = collisionWorld
                .Bodies[hit.RigidBodyIndex]
                .Collider.Value
                .GetCollisionFilter(hit.ColliderKey)
                .BelongsTo) {
                
                case PhysicsLayerHelper.Actor:
                    if (SystemAPI.GetComponent<TeamTypeData>(hit.Entity).teamType
                     == ownChampQuery.GetSingleton<TeamTypeData>().teamType)
                        castData.SetHitAlly(hit.Entity);
                    else castData.SetHitEnemy(hit.Entity);
                    break;
                
                case PhysicsLayerHelper.Ground: 
                    castData.SetHitGroundAt(hit.Position.Quantizate3()); 
                    break;
            }

            if ((totalHitLayer |= hitLayer) == GROUND_ACTOR) 
                break;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {
        castResult.Dispose();
    }
}