using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PlayerInputUpdateSystemGroup))]
public partial struct PlayerInputUpdateSystem : ISystem {
    private EntityQuery ownChampQuery;
    private Entity      prevHighlightedEntity;

    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<InputDirtyData>();
        state.RequireForUpdate<MouseCastData>();
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
        var     castResult     = SystemAPI.GetSingleton<MouseCastData>();

        // RESET
        inputData.Reset();

        // CHECK MOVE
        if (castResult.isHitGround
         && InputDirtyData.ButtonState.Down == inputDirtyData.rightMouse)
            inputData.SetMove(castResult.groundPos);

        // CHECK HIGHLIGHT
        if (prevHighlightedEntity != castResult.actor) {
            SetHighlight(ref state, prevHighlightedEntity, false);
            prevHighlightedEntity = castResult.actor;
            SetHighlight(ref state, prevHighlightedEntity, true);
        }
    }

    private void SetHighlight(ref SystemState state, Entity entity, bool isHighlighted) {
        if (entity == Entity.Null) return;
        SystemAPI.SetComponent(entity, new HighlightData { isHighlighted = isHighlighted });
    }
}