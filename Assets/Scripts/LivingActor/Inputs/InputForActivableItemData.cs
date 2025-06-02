using Unity.Entities;
using Unity.Transforms;

public struct InputForActivableItemData {
    public float3_Q3  walkableGround;
    public Entity     directEntity;
    public float3_Q3  ground;
    public float3_Q3  ownerPos;
    public floatXZ_Q3 direction;

    public void UpdateAll(in InputCastData castData, in InputDirtyData dirtyData, LocalTransform playerTrans) {
        walkableGround = castData.walkableGroundPos;

        directEntity = castData.actor;

        var mouseVec = dirtyData.mouse_ray_end - dirtyData.mouse_ray_start;
        var delY     = playerTrans.Position.y  - dirtyData.mouse_ray_start.y;
        ground = (dirtyData.mouse_ray_start    + mouseVec * delY / mouseVec.y).Quantizate3();

        ownerPos = playerTrans.Position.Quantizate3();

        // Calc direction base on cur player plane (that is created from player's y pos and parallel to Oxz plane)
        direction = (ground - ownerPos).xz;
    }
}