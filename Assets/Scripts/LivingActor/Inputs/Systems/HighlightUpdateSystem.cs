using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(InputLocalUpdateSystemGroup))]
public partial struct HighlightUpdateSystem : ISystem {
    private Entity prevHighlightedEntity;

    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<InputCastData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var castResult = SystemAPI.GetSingleton<InputCastData>();

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