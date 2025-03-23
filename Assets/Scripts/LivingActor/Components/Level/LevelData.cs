using Unity.Entities;
using Unity.NetCode;

public struct LevelData : IComponentData {
    [GhostField] public int curLevel;
    [GhostField] public int curExp;
    [GhostField] public int requireExp;
}