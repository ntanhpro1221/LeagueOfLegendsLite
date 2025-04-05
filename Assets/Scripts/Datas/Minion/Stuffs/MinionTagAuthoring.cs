using Unity.Entities;
using UnityEngine;

public struct MinionTag : IComponentData {
    public MinionId id;
}

public class MinionTagAuthoring : MonoBehaviour {
    public MinionId id;

    private class Baker : Baker<MinionTagAuthoring> {
        public override void Bake(MinionTagAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MinionTag {
                id = authoring.id
            });
        }
    }
}