using System;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct MonsterLeaderData : IComponentData {
    /// <summary>
    /// Reason for its existence is it is up-to-dated.<br/>
    /// <see cref="MonsterMyUnderlingBuffer"/> will take some tick to underling actually spawn.<br/>
    /// </summary>
    public int underlingCount;
}

[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct MonsterMyUnderlingBuffer : IBufferElementData, IEquatable<Entity> {
    public Entity entity;

    public static implicit operator Entity(MonsterMyUnderlingBuffer source) => source.entity;

    public bool Equals(Entity other) => entity == other;
}

[RequireComponent(typeof(MonsterExtraAuthoring))]
public class MonsterLeaderAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<MonsterLeaderAuthoring> {
        public override void Bake(MonsterLeaderAuthoring authoring) {
            GetDynamicEntity(out var entity);

            var extra = authoring.GetComponent<MonsterExtraAuthoring>();
            AddComponent(entity, new MonsterLeaderData {
                underlingCount = extra.active
                    ? extra.monsters.Count
                    : 0
            });
            AddBuffer<MonsterMyUnderlingBuffer>(entity);
        }
    }
}