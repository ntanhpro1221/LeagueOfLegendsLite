using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(HandleStatsSystemGroup))]
public partial struct UpdateStatsSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new GetRawValueJob()
            .ScheduleParallel(state.Dependency);

        state.Dependency = new GetRawPerLevelValueJob()
            .ScheduleParallel(state.Dependency);

        state.Dependency = new ApplyStatBuffsJob()
            .ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct GetRawValueJob : IJobEntity {
        [BurstCompile]
        public void Execute(
            in  StatsData.Raw raw
          , ref StatsData     stats) =>
            stats.CopyFromRaw(raw);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct GetRawPerLevelValueJob : IJobEntity {
        [BurstCompile]
        public void Execute(
            in  StatsData.RawPerLevel rawPerLevel
          , in  LevelData             level
          , ref StatsData             stats) =>
            stats.ApplyLevel(rawPerLevel, level);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct ApplyStatBuffsJob : IJobEntity {
        [BurstCompile]
        public void Execute(
            ref StatsData          stats
          , in  StatBuffs.Receiver buffs) =>
            stats.ApplyBuffs(buffs);
    }
}