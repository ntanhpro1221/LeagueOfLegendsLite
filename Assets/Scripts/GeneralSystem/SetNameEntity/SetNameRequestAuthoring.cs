using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct SetNameRequest : IComponentData {
    public FixedString64Bytes name;
}

public class SetNameRequestAuthoring : MonoBehaviour {
    public string _Name;

    private class Baker : Baker<SetNameRequestAuthoring> {
        public override void Bake(SetNameRequestAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SetNameRequest {
                name = string.IsNullOrEmpty(authoring._Name)
                    ? authoring.name
                    : authoring._Name
            });
        }
    }
}