using Unity.Entities;
using UnityEngine;

public class ActorSharedStateAuthoring : MonoBehaviour {
    public class Baker : Baker<ActorSharedStateAuthoring> {
        public override void Bake(ActorSharedStateAuthoring authoring) {
            Entity entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            this.AddActorSharedState(entity);
        }
    }
}