using Unity.Entities;
using UnityEngine;

public struct MonsterTag : IComponentData {
    public MonsterId     id;
}

[RequireComponent(typeof(JungleTeamTypeAuthoring))]
public class MonsterTagAuthoring : MonoBehaviour {
    public MonsterId id;

    private class Baker : Baker<MonsterTagAuthoring> {
        public override void Bake(MonsterTagAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MonsterTag {
                id = authoring.id
            });
        }
    }
}