using Unity.Entities;
using Unity.NetCode;

public struct ItemActiveNewStateRequestData : IComponentData {
    [GhostField] public bool           haveRequest;
    [GhostField] public SlotItemId     item;
    [GhostField] public uint           cooldownTick;
    [GhostField] public ItemActiveCost cost;

    public void PushRequest(SlotItemId _item, uint _cooldownTick, in ItemActiveCost _cost) {
        haveRequest  = true;
        item         = _item;
        cooldownTick = _cooldownTick;
        cost         = _cost;
    }

    public void Reset() => haveRequest = false;
}