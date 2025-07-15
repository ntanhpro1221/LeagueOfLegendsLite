using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct PlayerInputData : IInputComponentData {
    public void ResetAllEvents() => triggers.Event = default;

    [GhostField] public InputForActivableItemData              inputForActivableItem;
    [GhostField] public Strum.ItemActiveCond.Fields<bool> curCondition;
    [GhostField] public PlayerTrigger.Full                     triggers;
    [GhostField] public RequestData                            requestData;

    #region MOVE

    public void SetMove(float3_Q3 _targetLocalPos) {
        requestData.moveLocTarget = _targetLocalPos;
        triggers.Set(InputRequestId.Move);
    }

    public void CancelMove() {
        triggers.Set(InputRequestId.CancelMove);
    }

    #endregion

    #region ATTACK

    /// <summary>
    /// - Not in <see cref="RequestData"/> because this is not considered as request data, this is continuous value.<br/>
    /// - There is no request key for this in <see cref="InputRequestId"/>.<br/>
    /// </summary>
    [GhostField] public Entity attackTarget;

    public void SetAttack(Entity target) => attackTarget = target;

    public void CancelAttack() => attackTarget = Entity.Null;

    #endregion

    public struct RequestData {
        public ItemId     itemToBuy;
        public SlotItemId itemSlotToSell;
        public SlotItemId itemSlotToMove;
        public SlotItemId itemSlotMoveTarget;
        public SlotItemId skillToUpgrade;
        public float3_Q3  moveLocTarget;
    }
}

[RequireComponent(typeof(MoveableAuthoring))]
[RequireComponent(typeof(NormalAttackableAuthoring))]
public class PlayerInputAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<PlayerInputAuthoring> {
        public override void Bake(PlayerInputAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent<PlayerTrigger.PrevCode>(entity);
            AddComponent<PlayerInputData>(entity);
        }
    }
}