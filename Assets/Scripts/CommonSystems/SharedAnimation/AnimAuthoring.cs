using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct AnimData : IComponentData {
    [GhostField] public SharedAnimKey curAnim;
}

public class AnimAuthoring : MonoBehaviour {
    public SharedAnimKey entryAnim;

    private class Baker : Baker<AnimAuthoring> {
        public override void Bake(AnimAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AnimData {
                curAnim = authoring.entryAnim
            });
        }
    }
}