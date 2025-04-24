using Unity.Entities;
using Unity.NetCode;

[GhostEnabledBit]
public struct DestroyAtTick : IComponentData, IEnableableComponent {
    [GhostField] public NetworkTick tick;
}