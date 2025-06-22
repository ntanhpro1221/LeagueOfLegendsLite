using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct PlayerInputData : IInputComponentData {
    public void ResetAllEvents() {
        triggers.Event = default;
    }

    [GhostField] public InputForActivableItemData inputForActivableItem;
    [GhostField] public ItemActiveCondition       curCondition;
    [GhostField] public PlayerTrigger.Full        triggers;
    [GhostField] public RequestData               requestData;

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

    public void SetAttack(Entity target) => requestData.attackTarget = target;

    public void CancelAttack() => requestData.attackTarget = Entity.Null;

    #endregion

    public struct RequestData {
        public ItemId     itemToBuy;
        public SlotItemId itemSlotToSell;
        public SlotItemId itemSlotToMove;
        public SlotItemId itemSlotMoveTarget;
        public SlotItemId skillToUpgrade;
        public float3_Q3  moveLocTarget;
        public Entity     attackTarget;
    }
}

[GhostEnabledBit]
public struct PlayerInputResetting : IComponentData, IEnableableComponent { }

[RequireComponent(typeof(MoveableAuthoring))]
[RequireComponent(typeof(NormalAttackableAuthoring))]
public class PlayerInputAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<PlayerInputAuthoring> {
        public override void Bake(PlayerInputAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent<PlayerTrigger.PrevCode>(entity);
            AddComponent<PlayerInputData>(entity);
            AddComponent<PlayerInputResetting>(entity);
        }
    }
}