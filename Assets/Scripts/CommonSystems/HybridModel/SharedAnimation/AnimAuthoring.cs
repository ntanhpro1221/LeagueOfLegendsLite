using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct SharedAnimData : IComponentData {
    [GhostField] public SharedAnimKey curAnim;

    public bool isNeedRestart;

    public void MarkNeedRestart() => isNeedRestart = true;
}

public class AnimAuthoring : MonoBehaviour {
    public SharedAnimKey entryAnim;

    private class Baker : Baker<AnimAuthoring> {
        public override void Bake(AnimAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SharedAnimData {
                curAnim = authoring.entryAnim
            });
        }
    }
}