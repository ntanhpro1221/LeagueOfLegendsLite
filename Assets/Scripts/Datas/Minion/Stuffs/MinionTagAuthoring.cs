using Unity.Entities;
using UnityEngine;

public struct MinionTag : IComponentData {
    public MinionId id;
}

public class MinionTagAuthoring : MonoBehaviour, IRaceTag {
    public MinionId id;

    public int    TagInt => (int)id;
    public RaceId Race   => RaceId.Minion;

    private class Baker : Baker<MinionTagAuthoring> {
        public override void Bake(MinionTagAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MinionTag {
                id = authoring.id
            });
        }
    }
}