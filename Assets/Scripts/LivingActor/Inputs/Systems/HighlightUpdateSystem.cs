using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(InputLocalUpdateSystemGroup))]
public partial struct HighlightUpdateSystem : ISystem {
    private Entity curHighlighted;

    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<InputCastData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var castResult = SystemAPI.GetSingleton<InputCastData>();

        // UPDATE HIGHLIGHT
        if (curHighlighted != castResult.actor) {
            SetHighlight(ref state, curHighlighted, false);
            curHighlighted = castResult.actor;
            SetHighlight(ref state, curHighlighted, true);
        }
    }

    private void SetHighlight(ref SystemState state, Entity entity, bool isHighlighted) {
        if (entity == Entity.Null) return;
        SystemAPI.SetComponent(entity, new HighlightData(isHighlighted));
    }
}