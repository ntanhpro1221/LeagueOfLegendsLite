using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(HandleStatsSystemGroup))]
public partial struct InitAndUpdateStatsSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new EnableStatsJob()
            .ScheduleParallel(state.Dependency);

        state.Dependency = new GetRawValueJob()
            .ScheduleParallel(state.Dependency);

        state.Dependency = new GetRawPerLevelValueJob()
            .ScheduleParallel(state.Dependency);

        state.Dependency = new ApplyStatBuffsJob()
            .ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(StatsData_Raw))]
    [WithDisabled(
        typeof(StatsData))]
    [BurstCompile]
    private partial struct EnableStatsJob : IJobEntity {
        [BurstCompile]
        public void Execute(EnabledRefRW<StatsData> statsTrigger) {
            statsTrigger.ValueRW = true;
        }
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct GetRawValueJob : IJobEntity {
        [BurstCompile]
        public void Execute(
            in  StatsData_Raw statsRaw
          , ref StatsData     stats) {
            stats.data = statsRaw.data;
        }
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct GetRawPerLevelValueJob : IJobEntity {
        [BurstCompile]
        public void Execute(
            in  StatsData_RawPerLevel statsRawPerLevel
          , in  LevelData             level
          , ref StatsData             stats) {

            ref readonly var statsRawPerLevelData = ref statsRawPerLevel.data;

            ref var statsData = ref stats.data;

            foreach (var index in Strum.Stats.Info.Indexes)
                statsData[index] += statsRawPerLevelData[index] * (level.curLevel - 1);
        }
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct ApplyStatBuffsJob : IJobEntity {
        [BurstCompile]
        public void Execute(
            ref StatsData          stats
          , in  StatBuffs.Receiver buffs) {

            ref var statsData = ref stats.data;

            ref readonly var buffsData = ref buffs.buffs;

            foreach (var index in Strum.Stats.Info.Indexes)
                buffsData[index].ApplyTo(ref statsData.ValueRW(index));
        }
    }
}