using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct AimedTargetData : IComponentData {
    [GhostField] public Entity target;
    [GhostField] public bool   targetIsChampion;
}

public class AimedTargetAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<AimedTargetAuthoring> {
        public override void Bake(AimedTargetAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent<AimedTargetData>(entity);
        }
    }
}