using Unity.Entities;
using UnityEngine;

/// <summary>
/// entity with this component will be deleted automatically by <see cref="AutoDeleteEntitySystem"/>
/// </summary>
public struct AutoDeleteTag : IComponentData { }

public class DeleteEntityAuthoring : MonoBehaviour {
    public GameObject target;

    private class Baker : Baker<DeleteEntityAuthoring> {
        public override void Bake(DeleteEntityAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<AutoDeleteTag>(entity);
        }
    }
}