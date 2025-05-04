using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct AimedTargetData : IComponentData {
    [GhostField] public Entity target;
    
    [MarshalAs(UnmanagedType.U1)]
    [GhostField] public bool   targetIsChampion;
}

public class AimedTargetAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<AimedTargetAuthoring> {
        public override void Bake(AimedTargetAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent<AimedTargetData>(entity);
        }
    }
}