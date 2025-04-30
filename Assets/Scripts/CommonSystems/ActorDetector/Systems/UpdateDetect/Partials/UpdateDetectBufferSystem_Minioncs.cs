using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

public partial struct UpdateDetectedActorSystem {
    private void InitBuffer_Minion(ref SystemState state) {
        mainJob.data.MinionLookup = SystemAPI.GetComponentLookup<MinionTag>(
            isReadOnly: true);

        mainJob.data.detectedMinionLookup = SystemAPI.GetBufferLookup<DetectedMinionBuffer>(
            isReadOnly: false);
        mainJob.data.detectedByMinionLookup = SystemAPI.GetBufferLookup<DetectedByMinionBuffer>(
            isReadOnly: false);
    }

    private void ScheduleClearBuffer_Minion(ref SystemState state) {
        state.Dependency = new ClearDetectedMinionJob()
            .ScheduleParallel(state.Dependency);

        state.Dependency = new ClearDetectedByMinionJob()
            .ScheduleParallel(state.Dependency);
    }

    private void UpdateData_Minion(ref SystemState state) {
        mainJob.data.MinionLookup.Update(ref state);

        mainJob.data.detectedMinionLookup.Update(ref state);
        mainJob.data.detectedByMinionLookup.Update(ref state);
    }

    private partial struct MainJob {
        private void AppendToBuffer_Minion(in Entity detector, in Entity target) {
            if (data.MinionLookup.HasComponent(target)
             && data.detectedMinionLookup.TryGetBuffer(detector, out var detectedBuffer))
                detectedBuffer.Add(target);

            if (data.MinionLookup.HasComponent(detector)
             && data.detectedByMinionLookup.TryGetBuffer(target, out var detectByBuffer))
                detectByBuffer.Add(detector);
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

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct ClearDetectedByMinionJob : IJobEntity {
        [BurstCompile]
        public void Execute(ref DynamicBuffer<DetectedByMinionBuffer> detectedByBuffer) {
            detectedByBuffer.Clear();
        }
    }
}