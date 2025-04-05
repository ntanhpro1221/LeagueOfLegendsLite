using Unity.Entities;
using UnityEngine;

public class LivingActorSharedStateAuthoring : MonoBehaviour {
    public class Baker : Baker<LivingActorSharedStateAuthoring> {
        public override void Bake(LivingActorSharedStateAuthoring authoring) {
            Entity entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            this.AddChampionSharedState(entity);
        }
    }
}