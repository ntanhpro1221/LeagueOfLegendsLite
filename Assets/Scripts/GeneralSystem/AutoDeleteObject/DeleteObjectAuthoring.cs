using Unity.Entities;
using UnityEngine;

/// <summary>
/// <see cref="target"/> will be deleted automatically by <see cref="AutoDeleteObjectSystem"/>
/// </summary>
public struct DeleteObjectData : ICleanupComponentData {
    public UnityObjectRef<GameObject> target;
}

public class DeleteObjectAuthoring : MonoBehaviour {
    public GameObject target;

    private class Baker : Baker<DeleteObjectAuthoring> {
        public override void Bake(DeleteObjectAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DeleteObjectData {
                target = authoring.target
            });
        }
    }
}