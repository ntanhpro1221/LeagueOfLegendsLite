using Unity.Entities;
using UnityEngine;

public class ChampionSharedStateAuthoring : MonoBehaviour {
    public class Baker : Baker<ChampionSharedStateAuthoring> {
        public override void Bake(ChampionSharedStateAuthoring authoring) {
            Entity entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            this.AddChampionSharedState(entity);
        }
    }
}