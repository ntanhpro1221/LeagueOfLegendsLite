using System;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct CollidedOpponentBuffer : IBufferElementData, IEquatable<CollidedOpponentBuffer>, IEquatable<Entity> {
    [GhostField] public Entity entity;

    public bool Equals(CollidedOpponentBuffer other) {
        return entity.Equals(other.entity);
    }

    public bool Equals(Entity other) {
        return entity.Equals(other);
    }
}

public struct DamagedOpponentCount : IComponentData {
    [GhostField] public int count;
}

[RequireComponent(typeof(Collider))]
public class DamagedOpponentAuthoring : MonoBehaviour {
    private class Baker : Baker<DamagedOpponentAuthoring> {
        public override void Bake(DamagedOpponentAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddBuffer<CollidedOpponentBuffer>(entity);
            AddComponent<DamagedOpponentCount>(entity);
        }
    }
}