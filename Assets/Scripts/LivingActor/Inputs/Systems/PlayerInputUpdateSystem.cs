using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PlayerInputUpdateSystemGroup))]
public partial struct PlayerInputUpdateSystem : ISystem {
    private EntityQuery ownChampQuery;
    private Entity      prevHighlightedEntity;

    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<InputDirtyData>();
        state.RequireForUpdate<InputCastData>();
        state.RequireForUpdate(ownChampQuery = SystemAPI.QueryBuilder()
            .WithAll<ChampionTag, GhostOwnerIsLocal>()
            .WithAllRW<PlayerInputData>()
            .WithNone<NeedInitTag>()
            .Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        ref var inputData      = ref ownChampQuery.GetSingletonRW<PlayerInputData>().ValueRW;
        var     inputDirtyData = SystemAPI.GetSingleton<InputDirtyData>();
        var     castResult     = SystemAPI.GetSingleton<InputCastData>();

        // RESET
        inputData.Reset();

        // CHECK MOVE
        if (CheckMoveEvent(inputDirtyData, castResult)) {
            inputData.SetMove(castResult.groundPos);
            inputData.SetAttack(Entity.Null);
        }

        // CHECK HIGHLIGHT
        if (prevHighlightedEntity != castResult.actor) {
            SetHighlight(ref state, prevHighlightedEntity, false);
            prevHighlightedEntity = castResult.actor;
            SetHighlight(ref state, prevHighlightedEntity, true);
        }

        // CHECK ATTACK
        if (castResult.isHitActor)
            if ( // Left click
                inputDirtyData.leftMouse.WasPressedThisFrame()
                // Release A_Key
             || inputDirtyData.a_key.WasReleasedThisFrame())
                inputData.SetAttack(castResult.actor);

        // CANCEL MOVE AND ATTACK
        if (inputDirtyData.s_key.WasPressedThisFrame())
            inputData.CancelMoveAndAttack();
    }

    private void SetHighlight(ref SystemState state, Entity entity, bool isHighlighted) {
        if (entity == Entity.Null) return;
        SystemAPI.SetComponent(entity, new HighlightData { isHighlighted = isHighlighted });
    }

    public static bool CheckMoveEvent(in InputDirtyData dirtyData, in InputCastData castData) =>
        castData.isHitGround
     && dirtyData.rightMouse.WasPressedThisFrame();
}