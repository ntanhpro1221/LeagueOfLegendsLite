using Unity.Burst;
using Unity.Entities;

public partial struct UpdateDetectedActorSystem {
    private void InitBuffer_Monster(ref SystemState state) {
        mainJob.data.MonsterLookup = SystemAPI.GetComponentLookup<MonsterTag>(
            isReadOnly: true);

        mainJob.data.detectedMonsterLookup = SystemAPI.GetBufferLookup<DetectedMonsterBuffer>(
            isReadOnly: false);
    }

    private void ScheduleClearBuffer_Monster(ref SystemState state) {
        state.Dependency = new ClearDetectedMonsterJob()
            .ScheduleParallel(state.Dependency);
    }

    private void UpdateData_Monster(ref SystemState state) {
        mainJob.data.MonsterLookup.Update(ref state);

        mainJob.data.detectedMonsterLookup.Update(ref state);
    }

    private partial struct MainJob {
        private void AppendToBuffer_Monster(in Entity detector, in Entity target) {
            if (data.MonsterLookup.HasComponent(target)
             && data.detectedMonsterLookup.TryGetBuffer(detector, out var detectedBuffer))
                detectedBuffer.Add(target);
        }
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct ClearDetectedMonsterJob : IJobEntity {
        [BurstCompile]
        public void Execute(ref DynamicBuffer<DetectedMonsterBuffer> detectedBuffer) {
            detectedBuffer.Clear();
        }
    }
}