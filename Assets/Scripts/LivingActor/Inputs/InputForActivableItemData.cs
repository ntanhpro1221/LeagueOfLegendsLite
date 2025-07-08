using Unity.Entities;
using Unity.Transforms;

public struct InputForActivableItemData {
    /// <summary>
    /// Walkable ground position from player's cursor when activating.
    /// </summary>
    public float3_Q3 walkableGround;

    /// <summary>
    /// Direct entity that player point to when activating.
    /// </summary>
    public Entity directEntity;

    /// <summary>
    /// Ground position from player's cursor when activating.
    /// </summary>
    public float3_Q3 ground;

    /// <summary>
    /// Owner position when activating.
    /// </summary>
    public float3_Q3 ownerPos;

    /// <summary>
    /// Direction from owner to player's cursor when activating.
    /// </summary>
    public floatXZ_Q3 direction;

    public void UpdateAll(in InputCastData castData, in InputDirtyData dirtyData, in LocalTransform playerTrans) {
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