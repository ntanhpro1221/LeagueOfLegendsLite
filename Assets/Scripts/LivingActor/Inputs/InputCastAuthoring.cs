using System.Runtime.InteropServices;
using Unity.Entities;
using UnityEngine;

public struct InputCastData : IComponentData {
    #region GROUND

    public float3_Q3 groundPos;
    public float3_Q3 walkableGroundPos;
    public Entity    closestEntityAtGroundHit;

    [field: MarshalAs(UnmanagedType.U1)]
    public bool isHitGround { get; private set; }

    [field: MarshalAs(UnmanagedType.U1)]
    public bool isHitWalkableGround { get; private set; }
    
    [field: MarshalAs(UnmanagedType.U1)]
    public bool isHitClosestEntityAtGroundHit { get; private set; }

    public void SetHitGroundAt(float3_Q3 pos) => 
        (isHitGround, groundPos) = (true, pos);

    public void SetHitWalkableGroundAt(float3_Q3 pos) => 
        (isHitWalkableGround, walkableGroundPos) = (true, pos);

    public void SetClosestEntityAtGroundHit(Entity _closestEntityAtGroundHit) =>
        (isHitClosestEntityAtGroundHit, closestEntityAtGroundHit) = (true, _closestEntityAtGroundHit);
    
    #endregion

    #region ACTOR

    public Entity actor;

    public bool isHitActor => isHitAlly || isHitEnemy;
    [field: MarshalAs(UnmanagedType.U1)]
    public bool isHitAlly  { get; private set; }
    [field: MarshalAs(UnmanagedType.U1)]
    public bool isHitEnemy { get; private set; }

    public void SetHitAlly(Entity  ally)  => (isHitAlly, actor) = (true, ally);
    public void SetHitEnemy(Entity enemy) => (isHitEnemy, actor) = (true, enemy);

    #endregion

    public void Reset() => this = new();
}

public class InputCastAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<InputCastAuthoring> {
        public override void Bake(InputCastAuthoring authoring) {
            GetDynamicEntity(out var entity);
            
            AddComponent<InputCastData>(entity);
        }
    }
}