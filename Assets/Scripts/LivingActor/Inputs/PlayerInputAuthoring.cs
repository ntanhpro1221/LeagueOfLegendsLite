using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(OwnerSendType = SendToOwnerType.SendToNonOwner)]
public struct PlayerInputData : IInputComponentData {
    #region GENERAL

    private static readonly InputEvent TriggeredEvent = new() { Count = 1 };

    public void Reset() => this = new PlayerInputData();

    public void CancelMoveAndAttack() {
        cancelMoveEvent = TriggeredEvent;
        SetAttack(Entity.Null);
    }

    #endregion

    #region MOVE

    [GhostField] public float3_Q3  moveLocalTarget;
    [GhostField] public InputEvent moveEvent;
    [GhostField] public InputEvent cancelMoveEvent;

    public void SetMove(float3_Q3 _targetLocalPos)
        => (moveLocalTarget, moveEvent) = (_targetLocalPos, TriggeredEvent);

    #endregion

    #region ATTACK

    [GhostField] public Entity     attackTarget;
    [GhostField] public InputEvent attackEvent;

    public void SetAttack(Entity target)
        => (attackTarget, attackEvent) = (target, TriggeredEvent);

    #endregion
}

[RequireComponent(typeof(MoveableAuthoring))]
[RequireComponent(typeof(NormalAttackableAuthoring))]
public class PlayerInputAuthoring : MonoBehaviour {
    private class Baker : Baker<PlayerInputAuthoring> {
        public override void Bake(PlayerInputAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerInputData>(entity);
        }
    }
}