using Unity.Collections;
using Unity.Entities;

public partial struct UpdateDetectedActorSystem {
    private void InitBuffer_AllPartial(ref SystemState state) {
        mainJob.data.actorDetectorLookup = SystemAPI.GetComponentLookup<ActorDetector>(
            isReadOnly: true);
        mainJob.data.filterLookup = SystemAPI.GetComponentLookup<ActorDetectFilter>(
            isReadOnly: true);
        mainJob.data.teamLookup = SystemAPI.GetComponentLookup<TeamTypeData>(
            isReadOnly: true);
        
        InitBuffer_Champion(ref state);
        InitBuffer_Minion(ref state);
        InitBuffer_Monster(ref state);
        InitBuffer_Tower(ref state);
    }

    private void ScheduleClearBuffer_AllPartial(ref SystemState state) {
        ScheduleClearBuffer_Champion(ref state);
        ScheduleClearBuffer_Minion(ref state);
        ScheduleClearBuffer_Monster(ref state);
        ScheduleClearBuffer_Tower(ref state);
    }

    private void UpdateData_AllPartial(ref SystemState state) {
        mainJob.data.actorDetectorLookup.Update(ref state);
        mainJob.data.filterLookup.Update(ref state);
        mainJob.data.teamLookup.Update(ref state);

        UpdateData_Champion(ref state);
        UpdateData_Minion(ref state);
        UpdateData_Monster(ref state);
        UpdateData_Tower(ref state);
    }

    private partial struct MainJob {
        private void AppendToBuffer_AllPartial(in Entity detector, in Entity target) {
            AppendToBuffer_Champion(detector, target);
            AppendToBuffer_Minion(detector, target);
            AppendToBuffer_Monster(detector, target);
            AppendToBuffer_Tower(detector, target);
        }

        public struct Data {
            [ReadOnly] public ComponentLookup<ActorDetector>     actorDetectorLookup;
            [ReadOnly] public ComponentLookup<ActorDetectFilter> filterLookup;
            [ReadOnly] public ComponentLookup<TeamTypeData>      teamLookup;

            [ReadOnly] public ComponentLookup<ChampionTag>           ChampionLookup;
            public            BufferLookup<DetectedChampionBuffer>   detectedChampionLookup;

            [ReadOnly] public ComponentLookup<MinionTag>           MinionLookup;
            public            BufferLookup<DetectedMinionBuffer>   detectedMinionLookup;

            [ReadOnly] public ComponentLookup<MonsterTag>           MonsterLookup;
            public            BufferLookup<DetectedMonsterBuffer>   detectedMonsterLookup;

            [ReadOnly] public ComponentLookup<TowerTag>           TowerLookup;
            public            BufferLookup<DetectedTowerBuffer>   detectedTowerLookup;
        }
    }
}