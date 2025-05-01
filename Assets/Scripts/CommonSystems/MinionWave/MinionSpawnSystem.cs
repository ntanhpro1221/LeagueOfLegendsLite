using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
public partial struct MinionSpawnSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AllMinionWaveData>();
        state.RequireForUpdate<MinionWaveProcessBuffer>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var waveProcessBuffer = SystemAPI.GetSingletonBuffer<MinionWaveProcessBuffer>(
            isReadOnly: false);
        ref var allMinionData = ref SystemAPI.GetSingleton<AllMinionWaveData>().Data.Value.Values;

        for (int i = 0; i < waveProcessBuffer.Length; i++)
            HandleWaveProcess(waveProcessBuffer[i], ref allMinionData[i]);
    }

    [BurstCompile]
    public void HandleWaveProcess(in MinionWaveProcessBuffer waveProcess, ref MinionWaveData waveData) {
        // waveData.isFixedSpawn
    }
}