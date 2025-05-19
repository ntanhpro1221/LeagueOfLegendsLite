using Unity.Entities;

public struct ChampionPrefabBuffer : IPrefabBuffer {
    public Entity Entity { get; set; }

    public static implicit operator Entity(ChampionPrefabBuffer source) => source.Entity;
}

public struct MinionPrefabBuffer : IPrefabBuffer {
    public Entity Entity { get; set; }

    public static implicit operator Entity(MinionPrefabBuffer source) => source.Entity;
}

public struct MonsterPrefabBuffer : IPrefabBuffer {
    public Entity Entity { get; set; }
    
    public static implicit operator Entity(MonsterPrefabBuffer source) => source.Entity;
}

public struct TowerPrefabBuffer : IPrefabBuffer {
    public Entity Entity { get; set; }

    public static implicit operator Entity(TowerPrefabBuffer source) => source.Entity;
}
