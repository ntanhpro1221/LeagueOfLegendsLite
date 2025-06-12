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

[RequireComponent(
    typeof(Collider)
  , typeof(TeamTypeAuthoring))]
public class CollidedOpponentAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<CollidedOpponentAuthoring> {
        public override void Bake(CollidedOpponentAuthoring authoring) {
            GetDynamicEntity(out var entity);
            AddBuffer<CollidedOpponentBuffer>(entity);
        }
    }
}