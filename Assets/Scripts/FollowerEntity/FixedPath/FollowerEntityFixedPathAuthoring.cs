using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct FollowerEntityFixedPathBuffer : IBufferElementData {
    public float3 pos;
}

[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct FollowerEntityFixedPathStatus : IComponentData {
    public int curTargetIndex;
}

public class FollowerEntityFixedPathAuthoring : MonoBehaviour {
    public List<Transform> path;

    private class Baker : ExtendBaker<FollowerEntityFixedPathAuthoring> {
        public override void Bake(FollowerEntityFixedPathAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent<FollowerEntityFixedPathStatus>(entity);
            var buffer = AddBuffer<FollowerEntityFixedPathBuffer>(entity);
            foreach (var point in authoring.path)
                buffer.Add(new FollowerEntityFixedPathBuffer { pos = point.position });
        }
    }
}