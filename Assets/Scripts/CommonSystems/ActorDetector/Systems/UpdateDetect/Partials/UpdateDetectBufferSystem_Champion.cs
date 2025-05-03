using Unity.Burst;
using Unity.Entities;

public partial struct UpdateDetectedActorSystem {
    private void InitBuffer_Champion(ref SystemState state) {
        mainJob.data.ChampionLookup = SystemAPI.GetComponentLookup<ChampionTag>(
            isReadOnly: true);

        mainJob.data.detectedChampionLookup = SystemAPI.GetBufferLookup<DetectedChampionBuffer>(
            isReadOnly: false);
    }

    private void ScheduleClearBuffer_Champion(ref SystemState state) {
        state.Dependency = new ClearDetectedChampionJob()
            .ScheduleParallel(state.Dependency);
    }

    private void UpdateData_Champion(ref SystemState state) {
        mainJob.data.ChampionLookup.Update(ref state);

        mainJob.data.detectedChampionLookup.Update(ref state);
    }

    private partial struct MainJob {
        private void AppendToBuffer_Champion(in Entity detector, in Entity target) {
            if (data.ChampionLookup.HasComponent(target)
             && data.detectedChampionLookup.TryGetBuffer(detector, out var detectedBuffer))
                detectedBuffer.Add(target);
        }
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct ClearDetectedChampionJob : IJobEntity {
        [BurstCompile]
        public void Execute(ref DynamicBuffer<DetectedChampionBuffer> detectedBuffer) {
            detectedBuffer.Clear();
        }
    }
}