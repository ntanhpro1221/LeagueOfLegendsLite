using Unity.Entities;
using UnityEngine;

public class LevelAuthoring : MonoBehaviour {
    public int initLevel = 1;

    private class Baker : Baker<LevelAuthoring> {
        public override void Bake(LevelAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new LevelData {
                curLevel = authoring.initLevel
            });
        }
    }
}