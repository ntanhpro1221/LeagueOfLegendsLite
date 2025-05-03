using Pathfinding;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Not run in <see cref="InputCastUpdateSystem"/> because of burst
/// </summary>
[UpdateInGroup(typeof(BeforeInputLocalUpdateSystemGroup))]
[UpdateAfter(typeof(InputCastUpdateSystem))]
public partial struct InputCastNearestWalkableGroundSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<InputDirtyData>();
    }

    public void OnUpdate(ref SystemState state) {
        ref var castData = ref SystemAPI.GetSingletonRW<InputCastData>().ValueRW;

        if (!castData.isHitGround) return;

        var nnResult = AstarPath.active.GetNearest(castData.groundPos, NNConstraintHub.ClosestAsSeenFromAbove).position;
        if (nnResult.IsPositiveInfinity_X()) return;

        castData.SetHitWalkableGroundAt(nnResult.Quantizate3());
    }
}