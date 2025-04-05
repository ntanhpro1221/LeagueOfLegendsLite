using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial class PlayerInputSystem : SystemBase {
    private static GlobalInputAction Input;
    private static EntityQuery OwnChampQuery;
    private static readonly CollisionFilter Filter = new() {
        BelongsTo    = PhysicsLayerHelper.GroundRay
      , CollidesWith = PhysicsLayerHelper.Ground
    };

    #region CORE
    
    private bool IsMoveRequested(out float3_Q3 targetPos) {
        targetPos = default;
        
        // USER CLICK
        if (!Input.InGame.Click.triggered) 
            return false;

        // RAYCAST HIT THE GROUND
        var clickRay       = Camera.main!.ScreenPointToRay(Mouse.current.position.value);
        var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        if (!collisionWorld.CastRay(
            new RaycastInput {
                Start  = clickRay.origin
              , End    = clickRay.GetPoint(9999)
              , Filter = Filter
            }
          , out var hit)) 
          return false;
        
        targetPos = (float3_Q3)hit.Position;
        return true;
    }

    protected override void OnUpdate() {
        if (!OwnChampQuery.HasSingleton<PlayerInputData>()) return;
        ref var inputData = ref OwnChampQuery.GetSingletonRW<PlayerInputData>().ValueRW;
        
        // RESET
        inputData.Reset();

        // CHECK MOVE
        if (IsMoveRequested(out var targetLocalPos))
            inputData.SetMove(targetLocalPos);
    }
    
    #endregion

    protected override void OnCreate() {
        base.OnCreate();
        OwnChampQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<ChampionTag, GhostOwnerIsLocal>()
            .WithAllRW<PlayerInputData>()
            .WithNone<NeedInitTag>()
            .Build(EntityManager);
        
        Input = new();
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