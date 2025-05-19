using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct MonsterExtraBuffer : IBufferElementData {
    public MonsterId     id;
    public InitTransform initTrans;

    [Serializable]
    public class Managed {
        public MonsterId id;
        public Transform initTrans;
    }
}

[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct MonsterExtraBufferCount : IComponentData {
    public int Count;
}

[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct MonsterExtraTrigger : IComponentData, IEnableableComponent { }

/// <summary>
/// Only work with <see cref="MonsterLeaderAuthoring"/>
/// </summary>
[Tooltip("Only work with leader of monster camp\nIf you are not leader, just use it as a database, dont active it")]
public class MonsterExtraAuthoring : MonoBehaviour {
    public bool                             active;
    public List<MonsterExtraBuffer.Managed> monsters;

    private class Baker : ExtendBaker<MonsterExtraAuthoring> {
        public override void Bake(MonsterExtraAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent<MonsterExtraTrigger>(entity);
            SetComponentEnabled<MonsterExtraTrigger>(entity, authoring.active);

            AddComponent(entity, new MonsterExtraBufferCount { Count = authoring.monsters.Count });
            
            var buffer = AddBuffer<MonsterExtraBuffer>(entity);
            foreach (var monster in authoring.monsters)
                buffer.Add(new MonsterExtraBuffer {
                    id        = monster.id
                  , initTrans = monster.initTrans
                });
        }
    }
}