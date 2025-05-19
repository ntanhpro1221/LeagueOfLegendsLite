using Unity.Burst;
using Unity.Entities;
using UnityEngine;

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
            SetHighlight(ref state, false);
            curHighlighted = castResult.actor;
            SetHighlight(ref state, true);
        }
    }

    private void SetHighlight(ref SystemState state, bool isHighlighted) {
        if (!SystemAPI.Exists(curHighlighted)
            // May have cleanup component
         || !SystemAPI.HasComponent<HighlightData>(curHighlighted)) return;
        SystemAPI.SetComponent(curHighlighted, new HighlightData(isHighlighted));
    }
}