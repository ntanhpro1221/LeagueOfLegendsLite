using Unity.Entities;

public struct ChampionPrefabBuffer : IPrefabBuffer { public Entity Entity { get; set; } }
public struct MinionPrefabBuffer : IPrefabBuffer { public Entity Entity { get; set; } }
public struct MonsterPrefabBuffer : IPrefabBuffer { public Entity Entity { get; set; } }
public struct TowerPrefabBuffer : IPrefabBuffer { public Entity Entity { get; set; } }
