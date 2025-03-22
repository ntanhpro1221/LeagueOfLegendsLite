using Unity.Entities;
using UnityEngine;

public struct ChampionTag : IComponentData { }

public class ChampionTagAuthoring : MonoBehaviour {
    private class Baker : Baker<ChampionTagAuthoring> {
        public override void Bake(ChampionTagAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<ChampionTag>(entity);
        }
    }
}