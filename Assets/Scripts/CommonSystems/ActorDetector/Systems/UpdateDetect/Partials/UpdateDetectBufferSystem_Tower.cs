using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

public partial struct UpdateDetectedActorSystem {
    private void InitBuffer_Tower(ref SystemState state) {
        mainJob.data.TowerLookup = SystemAPI.GetComponentLookup<TowerTag>(
            isReadOnly: true);

        mainJob.data.detectedTowerLookup = SystemAPI.GetBufferLookup<DetectedTowerBuffer>(
            isReadOnly: false);
        mainJob.data.detectedByTowerLookup = SystemAPI.GetBufferLookup<DetectedByTowerBuffer>(
            isReadOnly: false);
    }

    private void ScheduleClearBuffer_Tower(ref SystemState state) {
        state.Dependency = new ClearDetectedTowerJob()
            .ScheduleParallel(state.Dependency);

        state.Dependency = new ClearDetectedByTowerJob()
            .ScheduleParallel(state.Dependency);
    }

    private void UpdateData_Tower(ref SystemState state) {
        mainJob.data.TowerLookup.Update(ref state);

        mainJob.data.detectedTowerLookup.Update(ref state);
        mainJob.data.detectedByTowerLookup.Update(ref state);
    }

    private partial struct MainJob {
        private void AppendToBuffer_Tower(in Entity detector, in Entity target) {
            if (data.TowerLookup.HasComponent(target)
             && data.detectedTowerLookup.TryGetBuffer(detector, out var detectedBuffer))
                detectedBuffer.Add(target);

            if (data.TowerLookup.HasComponent(detector)
             && data.detectedByTowerLookup.TryGetBuffer(target, out var detectByBuffer))
                detectByBuffer.Add(detector);
        }
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct ClearDetectedTowerJob : IJobEntity {
        [BurstCompile]
        public void Execute(ref DynamicBuffer<DetectedTowerBuffer> detectedBuffer) {
            detectedBuffer.Clear();
        }
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct ClearDetectedByTowerJob : IJobEntity {
        [BurstCompile]
        public void Execute(ref DynamicBuffer<DetectedByTowerBuffer> detectedByBuffer) {
            detectedByBuffer.Clear();
        }
    }
}