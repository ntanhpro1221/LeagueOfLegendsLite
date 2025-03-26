using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct NeedInitTag : IComponentData { }

public class NeedInitAuthoring : MonoBehaviour {
    private class Baker : Baker<NeedInitAuthoring> {
        public override void Bake(NeedInitAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<NeedInitTag>(entity);
        }
    }
}