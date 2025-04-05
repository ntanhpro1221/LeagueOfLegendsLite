using Unity.Entities;
using UnityEngine;

public struct TowerTag : IComponentData {
    public TowerId id;
}

public class TowerTagAuthoring : MonoBehaviour {
    public TowerId id;

    private class Baker : Baker<TowerTagAuthoring> {
        public override void Bake(TowerTagAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TowerTag {
                id = authoring.id
            });
        }
    }
}