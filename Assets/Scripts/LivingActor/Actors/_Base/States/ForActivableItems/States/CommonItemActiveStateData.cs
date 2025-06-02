using Unity.Entities;
using Unity.NetCode;

public struct CommonItemActiveStateData : IComponentData {
    [GhostField] public InputForActivableItemData inputForActive;

    [GhostField] public Perform performData;

    public void SetInputData(in PlayerInputData input) {
        inputForActive = input.inputForActivableItem;
    }

    /// <summary>
    /// For skill of throw something or perform special action
    /// </summary>
    public struct Perform {
        public NetworkTick performTick;
        public NetworkTick doneTick;
        public bool        isPerformed;

        public void Enter(NetworkTick _performTick, NetworkTick _doneTick) {
            performTick = _performTick;
            doneTick    = _doneTick;
            isPerformed = false;
        }

        public readonly bool IsReadyToPerform(NetworkTick curTick) =>
            curTick.IsNewerThan(performTick)
         && isPerformed == false;

        public void MarkPerformed() => isPerformed = true;
    }
}