using System;
using Unity.Entities;
using Unity.Entities.Hybrid.Baking;
using Unity.NetCode;
using UnityEngine;

public struct NeedSpawnActorDetector : IComponentData, IEnableableComponent {
    public Entity prefab;
}

public struct DetectedChampionBuffer : IBufferElementData {
    [GhostField] public Entity entity;

    public static implicit operator DetectedChampionBuffer(Entity entity)
        => new() { entity = entity };
}

public struct DetectedMinionBuffer : IBufferElementData {
    [GhostField] public Entity entity;

    public static implicit operator DetectedMinionBuffer(Entity entity)
        => new() { entity = entity };
}

public struct DetectedTowerBuffer : IBufferElementData {
    [GhostField] public Entity entity;

    public static implicit operator DetectedTowerBuffer(Entity entity)
        => new() { entity = entity };
}

public struct DetectedMonsterBuffer : IBufferElementData {
    [GhostField] public Entity entity;

    public static implicit operator DetectedMonsterBuffer(Entity entity)
        => new() { entity = entity };
}

[RequireComponent(typeof(LinkedEntityGroupAuthoring))] // To link actor detector
public class ActorDetectorHolderAuthoring : MonoBehaviour {
    public GameObject actorDetectorPrefab;
    public DetectTarget targets;

    private class Baker : ExtendBaker<ActorDetectorHolderAuthoring> {
        public override void Bake(ActorDetectorHolderAuthoring authoring) {
            GetDynamicEntity(out var entity);
            
            AddComponent(entity, new NeedSpawnActorDetector {
                prefab = GetDynamicEntity(authoring.actorDetectorPrefab)
            });
            
            if (authoring.targets.HasFlag(DetectTarget.Champion))
                AddBuffer<DetectedChampionBuffer>(entity);

            if (authoring.targets.HasFlag(DetectTarget.Minion))
                AddBuffer<DetectedMinionBuffer>(entity);

            if (authoring.targets.HasFlag(DetectTarget.Tower))
                AddBuffer<DetectedTowerBuffer>(entity);

            if (authoring.targets.HasFlag(DetectTarget.Monster))
                AddBuffer<DetectedMonsterBuffer>(entity);
        }
    }

    [Flags]
    public enum DetectTarget {
        Champion = 1 << 0
      , Minion   = 1 << 1
      , Tower    = 1 << 2
      , Monster  = 1 << 3
    }
}