using Unity.Entities;
using Unity.NetCode;

public struct ItemActiveRequestData : IComponentData {
    [GhostField] public bool           haveRequestNewState;
    [GhostField] public SlotItemId     item;
    [GhostField] public uint           cooldownTick;
    [GhostField] public ItemActiveCost cost;

    public void PushRequest(SlotItemId _item, uint _cooldownTick, in ItemActiveCost _cost, bool requireNewState) {
        haveRequestNewState = requireNewState;
        item                = _item;
        cooldownTick        = _cooldownTick;
        cost                = _cost;
    }
}