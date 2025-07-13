using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct ChampionTag : IComponentData {
    public ChampionId id;
}

public struct ChampOrderInTeam : IComponentData {
    [GhostField] public int order;
}

[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct ChampConnection : IComponentData {
    public Entity entity;
}

public class ChampionTagAuthoring : MonoBehaviour, IRaceTag {
    public ChampionId id;

    public int    TagInt => (int)id;
    public RaceId Race   => RaceId.Champ;

    private class Baker : Baker<ChampionTagAuthoring> {
        public override void Bake(ChampionTagAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ChampionTag { id = authoring.id });
            AddComponent<ChampOrderInTeam>(entity);
            AddComponent<ChampConnection>(entity);
        }
    }
}