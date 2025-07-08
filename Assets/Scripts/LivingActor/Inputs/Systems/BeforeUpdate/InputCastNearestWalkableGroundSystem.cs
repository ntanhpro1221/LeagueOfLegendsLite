using Unity.Burst;
using Unity.Entities;

/// <summary>
/// Not run in <see cref="InputCastUpdateSystem"/> because of burst
/// </summary>
[UpdateInGroup(typeof(BeforeInputLocalUpdateSystemGroup))]
[UpdateAfter(typeof(InputCastUpdateSystem))]
public partial struct InputCastNearestWalkableGroundSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<InputCastData>();
    }

    public void OnUpdate(ref SystemState state) {
        if (AstarPath.active == null) return;

        ref var castData = ref SystemAPI.GetSingletonRW<InputCastData>().ValueRW;

        if (!castData.isHitGround) return;

        var nnResult = AstarPath.active.GetNearest(castData.groundPos, NNConstraintHub.ClosestAsSeenFromAbove).position;
        if (nnResult.IsPositiveInfinity_X()) return;

        castData.SetHitWalkableGroundAt(nnResult.Quantizate3());
    }
}