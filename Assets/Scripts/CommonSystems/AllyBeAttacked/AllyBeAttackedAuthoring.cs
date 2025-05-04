using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct AllyBeAttackedData : IComponentData {
    [GhostField] public Entity champByChamp;
}

public class AllyBeAttackedAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<AllyBeAttackedAuthoring> {
        public override void Bake(AllyBeAttackedAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent<AllyBeAttackedData>(entity);
        }
    }
}