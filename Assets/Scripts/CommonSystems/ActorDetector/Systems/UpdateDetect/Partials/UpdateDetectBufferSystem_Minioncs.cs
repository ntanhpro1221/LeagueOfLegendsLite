using Unity.Burst;
using Unity.Entities;

public partial struct UpdateDetectedActorSystem {
    private void InitBuffer_Minion(ref SystemState state) {
        mainJob.data.MinionLookup = SystemAPI.GetComponentLookup<MinionTag>(
            isReadOnly: true);

        mainJob.data.detectedMinionLookup = SystemAPI.GetBufferLookup<DetectedMinionBuffer>(
            isReadOnly: false);
    }

    private void ScheduleClearBuffer_Minion(ref SystemState state) {
        state.Dependency = new ClearDetectedMinionJob()
            .ScheduleParallel(state.Dependency);
    }

    private void UpdateData_Minion(ref SystemState state) {
        mainJob.data.MinionLookup.Update(ref state);

        mainJob.data.detectedMinionLookup.Update(ref state);
    }

    private partial struct MainJob {
        private void AppendToBuffer_Minion(in Entity detector, in Entity target) {
            if (data.MinionLookup.HasComponent(target)
             && data.detectedMinionLookup.TryGetBuffer(detector, out var detectedBuffer))
                detectedBuffer.Add(target);
        }
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct ClearDetectedMinionJob : IJobEntity {
        [BurstCompile]
        public void Execute(ref DynamicBuffer<DetectedMinionBuffer> detectedBuffer) {
            detectedBuffer.Clear();
        }
    }
}