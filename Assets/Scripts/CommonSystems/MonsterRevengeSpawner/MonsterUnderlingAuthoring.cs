using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct MonsterUnderlingData : IComponentData {
    public Entity leader;
}

[RequireComponent(typeof(MonsterManualInitTransAndAnchorAuthoring))]
public class MonsterUnderlingAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<MonsterUnderlingAuthoring> {
        public override void Bake(MonsterUnderlingAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent<MonsterUnderlingData>(entity);
        }
    }
}