using Unity.Entities;

public struct MouseCastData : IComponentData {
    #region GROUND

    public float3_Q3 groundPos;

    public bool isHitGround { get; private set; }

    public void SetHitGroundAt(float3_Q3 pos) => (isHitGround, groundPos) = (true, pos);

    #endregion

    #region ACTOR

    public Entity actor;

    public bool isHitActor => isHitAlly || isHitEnemy;
    public bool isHitAlly  { get; private set; }
    public bool isHitEnemy { get; private set; }

    public void SetHitAlly(Entity  ally)  => (isHitAlly, actor) = (true, ally);
    public void SetHitEnemy(Entity enemy) => (isHitEnemy, actor) = (true, enemy);

    #endregion

    public void Reset() => this = new();
}