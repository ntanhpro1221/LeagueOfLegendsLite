using System;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct MinionWaveProcessBuffer : IBufferElementData {
    [GhostField] public int         spawnedWaveCnt;
    [GhostField] public int         spawnedMinionInWaveCnt;
    [GhostField] public NetworkTick nextMinionSpawnTick;
}

public class MinionSpawnerAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<MinionSpawnerAuthoring> {
        public override void Bake(MinionSpawnerAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddCleanBuffer<MinionWaveProcessBuffer>(
                entity
              , Enum.GetValues(typeof(MinionWaveType)).Length);
        }
    }
}