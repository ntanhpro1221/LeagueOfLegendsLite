using Unity.Entities;
using UnityEngine;

public struct ChampionTag : IComponentData {
    public ChampionId id;
}

public class ChampionTagAuthoring : MonoBehaviour {
    public ChampionId id;

    private class Baker : Baker<ChampionTagAuthoring> {
        public override void Bake(ChampionTagAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ChampionTag {
                id = authoring.id
            });
        }
    }
}