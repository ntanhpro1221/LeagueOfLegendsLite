using Unity.Entities;
using Unity.NetCode;

public struct AttackStateData : IComponentData {
    [GhostField] public NetworkTick cooldownDoneAtTick;
}