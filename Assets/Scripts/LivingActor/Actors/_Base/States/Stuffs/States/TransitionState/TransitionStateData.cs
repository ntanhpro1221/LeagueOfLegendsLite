using Unity.Entities;
using Unity.NetCode;

public struct TransitionStateData : IComponentData {
    [GhostField] public NetworkTick DoneAtTick;
}