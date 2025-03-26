using Unity.Entities;
using Unity.NetCode;

public struct DamageTargetData : IComponentData {
    [GhostField] public Entity target;
}