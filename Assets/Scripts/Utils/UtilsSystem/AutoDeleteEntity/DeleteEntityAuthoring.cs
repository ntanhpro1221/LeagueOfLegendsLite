using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// entity with this component will be deleted automatically by <see cref="AutoDeleteEntitySystem"/>
/// </summary>
public struct AutoDeleteTag : IComponentData {
    public WorldToDelete WorldToDelete;
}

public class DeleteEntityAuthoring : MonoBehaviour {
    public WorldToDelete WorldToDelete;

    private class Baker : Baker<DeleteEntityAuthoring> {
        public override void Bake(DeleteEntityAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AutoDeleteTag {
                WorldToDelete = authoring.WorldToDelete
            });
        }
    }
}