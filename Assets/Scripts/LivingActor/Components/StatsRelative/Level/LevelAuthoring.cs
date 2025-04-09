using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct LevelData : IComponentData {
    [GhostField] public int curLevel;
    [GhostField] public int curExp;
}

public struct IncomingExpBuffer : IBufferElementData {
    [GhostField] public int exp;
}

public class LevelAuthoring : MonoBehaviour {
    public int initLevel = 1;

    private class Baker : ExtendBaker<LevelAuthoring> {
        public override void Bake(LevelAuthoring authoring) {
            GetDynamicEntity(out var entity);
            
            AddComponent(entity, new LevelData { curLevel = authoring.initLevel });
            AddBuffer<IncomingExpBuffer>(entity);
        }
    }
}