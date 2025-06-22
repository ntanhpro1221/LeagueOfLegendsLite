using Unity.Entities;
using Unity.NetCode;

public struct ItemCommonStateData : IComponentData {
    [GhostField] public InputForActivableItemData input;

    [GhostField] public Perform performData;

    [GhostField] public SlotItemId itemSlot;

    public void SetData(in PlayerInputData input, SlotItemId slot) {
        this.input = input.inputForActivableItem;
        itemSlot = slot;
    }

    /// <summary>
    /// For skill of throw something or perform special action (such as normal attack).
    /// </summary>
    public struct Perform {
        public NetworkTick performTick;
        public NetworkTick doneTick;
        public bool        isPerformed;

        public void Enter(in NetworkTick curTick, uint lifeTick, float triggerPoint) {
            performTick = curTick.WithBonusTick((uint)(lifeTick * triggerPoint));
            doneTick    = curTick.WithBonusTick(lifeTick);
            isPerformed = false;
        }

        public readonly bool IsReadyToPerform(NetworkTick curTick) =>
            curTick.IsNewerThan(performTick)
         && isPerformed == false;

        public void MarkPerformed() => isPerformed = true;
    }
}