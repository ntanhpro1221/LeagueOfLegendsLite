using System.Runtime.InteropServices;
using Unity.Entities;

public struct InputCastData : IComponentData {
    #region GROUND

    public float3_Q3 groundPos;

    [field: MarshalAs(UnmanagedType.U1)]
    public bool isHitGround { get; private set; }

    public void SetHitGroundAt(float3_Q3 pos) => (isHitGround, groundPos) = (true, pos);

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