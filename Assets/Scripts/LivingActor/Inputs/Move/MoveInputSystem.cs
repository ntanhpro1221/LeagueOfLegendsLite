using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial class MoveInputSystem : SystemBase {
    private static GlobalInputAction Input;
    private static EntityQuery OwnChampQuery;
    private static readonly CollisionFilter Filter = new() {
        BelongsTo    = PhysicsLayerHelper.GroundRay
      , CollidesWith = PhysicsLayerHelper.Ground
    };

    private void OnClick(InputAction.CallbackContext context) {
        if (!OwnChampQuery.HasSingleton<MoveInputData>()) return;

        var clickRay       = Camera.main!.ScreenPointToRay(Mouse.current.position.value);
        var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

        if (collisionWorld.CastRay(
            new RaycastInput {
                Start  = clickRay.origin
              , End    = clickRay.GetPoint(9999)
              , Filter = Filter
            }
          , out Unity.Physics.RaycastHit hit))
            OwnChampQuery.GetSingletonRW<MoveInputData>().ValueRW.targetPos = hit.Position;
    }

    protected override void OnUpdate() { }
    
    protected override void OnCreate() {
        base.OnCreate();
        OwnChampQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<ChampionTag, GhostOwnerIsLocal>()
            .WithAllRW<MoveInputData>()
            .WithNone<ClientNeedInitTag>()
            .Build(EntityManager);
        
        Input = new();
        Input.InGame.Click.performed += OnClick;
    }
    
    protected override void OnStartRunning() {
        base.OnStartRunning();
        Input.Enable();
    }
    
    protected override void OnStopRunning() {
        base.OnStopRunning();
        Input.Disable();
    }
}