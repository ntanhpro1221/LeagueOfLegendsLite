using Unity.Entities;
using Unity.NetCode;

[GhostEnabledBit]
public struct DeadState : IComponentData, IEnableableComponent { }