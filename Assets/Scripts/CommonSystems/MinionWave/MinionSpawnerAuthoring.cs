using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.Assertions;

[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct MinionSpawnerData : IComponentData {
    [GhostField] public Entity      targetInhibitor;
    [GhostField] public NetworkTick nextWaveTick;
    [GhostField] public int         curWaveId;

    public uint waveInterval;
    public uint minionInterval;

    public void ToNextWave(int waveLoopSize) {
        curWaveId = (curWaveId + 1) % waveLoopSize;
        nextWaveTick.Add(waveInterval);
    }
}

[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct MinionSpawnQueueData : IBufferElementData {
    [GhostField] public MinionId minionId;
    [GhostField] public NetworkTick spawnTick;
}

[RequireComponent(
    typeof(LaneTypeAuthoring)
  , typeof(TeamTypeAuthoring))]
public class MinionSpawnerAuthoring : MonoBehaviour {
    public GameObject targetInhibitor;

    public NetCodeConfig netcodeConfig;

    [Min(0)]     public float firstWaveTime;
    [Min(10)]    public float waveInterval;
    [Min(0.05f)] public float minionInterval;

    private class Baker : ExtendBaker<MinionSpawnerAuthoring> {
        public override void Bake(MinionSpawnerAuthoring authoring) {
            GetDynamicEntity(out var entity);

            int tickRate = authoring.netcodeConfig.ClientServerTickRate.SimulationTickRate;

            var spawnerData = new MinionSpawnerData {
                targetInhibitor = GetDynamicEntity(authoring.targetInhibitor)
              , nextWaveTick    = TickHelpers.CalcStartTick(authoring.firstWaveTime, tickRate)
              , waveInterval    = TickHelpers.CountTick(authoring.waveInterval,   tickRate)
              , minionInterval  = TickHelpers.CountTick(authoring.minionInterval, tickRate)
            };
            AddComponent(entity, spawnerData);
            AddBuffer<MinionSpawnQueueData>(entity);

            // Safety check
            Assert.AreNotEqual(spawnerData.waveInterval, 0u
              , "NGDtuanh: interval between two wave is ZERO, this is invalid");
        }
    }
}