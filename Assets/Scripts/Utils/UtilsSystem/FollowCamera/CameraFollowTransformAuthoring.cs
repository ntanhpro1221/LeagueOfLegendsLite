using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct CameraFollowTransformData : IComponentData {
    public float3 delta;
}

public class CameraFollowTransformAuthoring : MonoBehaviour {
    public Vector3 delta;

    private class Baker : Baker<CameraFollowTransformAuthoring> {
        public override void Bake(CameraFollowTransformAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CameraFollowTransformData {
                delta = authoring.delta
            });
            AddComponent(entity, new AutoDeleteTag {
                WorldToDelete = WorldToDelete.Server
            });
        }
    }
}