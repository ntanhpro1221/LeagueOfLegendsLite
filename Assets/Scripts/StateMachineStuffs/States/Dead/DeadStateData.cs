using Unity.Entities;
using Unity.NetCode;

public struct DeadStateData : IComponentData {
    [GhostField] public NetworkTick respawnAtTick;
}