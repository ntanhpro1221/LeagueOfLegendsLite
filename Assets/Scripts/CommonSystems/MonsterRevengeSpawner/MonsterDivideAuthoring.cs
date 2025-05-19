using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

/// <summary>
/// Mostly active when monster dead
/// </summary>
[Serializable]
[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct MonsterDivideBuffer : IBufferElementData {
    public MonsterId id;
}

[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct MonsterDivideBufferCount : IComponentData {
    public int Count;
}

[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct MonsterDivideTrigger : IComponentData, IEnableableComponent { }

/// <summary>
/// Work with either <see cref="MonsterLeaderAuthoring"/> or <see cref="MonsterUnderlingAuthoring"/>
/// </summary>
[Tooltip("Only work with monster camp (you must be either leader or underling)")]
public class MonsterDivideAuthoring : MonoBehaviour {
    public List<MonsterDivideBuffer> monsters;

    private class Baker : ExtendBaker<MonsterDivideAuthoring> {
        public override void Bake(MonsterDivideAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddBuffer(entity, authoring.monsters);
            AddComponent(entity, new MonsterDivideBufferCount { Count = authoring.monsters.Count });
            AddComponentDisabled<MonsterDivideTrigger>(entity);
        }
    }
}