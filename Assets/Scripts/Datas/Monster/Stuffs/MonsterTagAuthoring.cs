using Unity.Entities;
using UnityEngine;

public struct MonsterTag : IComponentData {
    public MonsterId     id;
}

[RequireComponent(typeof(JungleTeamTypeAuthoring))]
public class MonsterTagAuthoring : MonoBehaviour, IRaceTag {
    public MonsterId id;

    public int    TagInt => (int)id;
    public RaceId Race   => RaceId.Monster;

    private class Baker : Baker<MonsterTagAuthoring> {
        public override void Bake(MonsterTagAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MonsterTag {
                id = authoring.id
            });
        }
    }
}