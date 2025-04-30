using System;
using Unity.Entities;
using Unity.Entities.Hybrid.Baking;
using Unity.NetCode;
using UnityEngine;

public struct DetectedByChampionBuffer : IBufferElementData {
    [GhostField] public Entity entity;

    public static implicit operator DetectedByChampionBuffer(Entity entity)
        => new() { entity = entity };
}

public struct DetectedByMinionBuffer : IBufferElementData {
    [GhostField] public Entity entity;

    public static implicit operator DetectedByMinionBuffer(Entity entity)
        => new() { entity = entity };
}

public struct DetectedByTowerBuffer : IBufferElementData {
    [GhostField] public Entity entity;

    public static implicit operator DetectedByTowerBuffer(Entity entity)
        => new() { entity = entity };
}

public struct DetectedByMonsterBuffer : IBufferElementData {
    [GhostField] public Entity entity;

    public static implicit operator DetectedByMonsterBuffer(Entity entity)
        => new() { entity = entity };
}

[RequireComponent(typeof(LinkedEntityGroupAuthoring))] // To link actor detector
public class ActorDetectedByAuthoring : MonoBehaviour {
    public DetectBy detectedBy;

    private class Baker : ExtendBaker<ActorDetectedByAuthoring> {
        public override void Bake(ActorDetectedByAuthoring authoring) {
            GetDynamicEntity(out var entity);

            if (authoring.detectedBy.HasFlag(DetectBy.Champion))
                AddBuffer<DetectedByChampionBuffer>(entity);

            if (authoring.detectedBy.HasFlag(DetectBy.Minion))
                AddBuffer<DetectedByMinionBuffer>(entity);

            if (authoring.detectedBy.HasFlag(DetectBy.Tower))
                AddBuffer<DetectedByTowerBuffer>(entity);

            if (authoring.detectedBy.HasFlag(DetectBy.Monster))
                AddBuffer<DetectedByMonsterBuffer>(entity);
        }
    }

    [Flags]
    public enum DetectBy {
        Champion = 1 << 0
      , Minion   = 1 << 1
      , Tower    = 1 << 2
      , Monster  = 1 << 3
    }
}