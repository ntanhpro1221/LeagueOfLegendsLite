using Unity.Entities;
using UnityEngine;

/// <summary>
/// target in <see cref="DeleteObjectData"/> will be deleted if this tag exist
/// </summary>
public struct DeleteObjectRequest : IComponentData {
    public bool deleteRequestEntity;
}

public class DeleteObjectAuthoring : MonoBehaviour {
    public bool deleteRequestEntity;

    private class Baker : Baker<DeleteObjectAuthoring> {
        public override void Bake(DeleteObjectAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DeleteObjectRequest {
                deleteRequestEntity = authoring.deleteRequestEntity
            });
        }
    }
}