using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

public struct MoveStateData : IComponentData {
    [GhostField(Quantization = 0)] public float3 destination;
}