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

        state.Dependency = new ApplyBuffJob()
            .ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(StatsBuffer_Raw))]
    [WithDisabled(
        typeof(StatsBuffer))]
    [BurstCompile]
    private partial struct EnableStatsJob : IJobEntity {
        [BurstCompile]
        public void Execute(EnabledRefRW<StatsBuffer> statsTrigger) {
            statsTrigger.ValueRW = true;
        }
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct GetRawValueJob : IJobEntity {
        [BurstCompile]
        public void Execute(
            in  DynamicBuffer<StatsBuffer_Raw> statsRaw
          , ref DynamicBuffer<StatsBuffer>     stats) {
            for (int i = 0; i < EnumCount.Stats; ++i) stats[i] = statsRaw[i].value;
        }
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct GetRawPerLevelValueJob : IJobEntity {
        [BurstCompile]
        public void Execute(
            in  DynamicBuffer<StatsBuffer_RawPerLevel> statsRawPerLevel
          , in  LevelData                              level
          , ref DynamicBuffer<StatsBuffer>             stats) {
            for (int i = 0; i < EnumCount.Stats; ++i)
                stats[i] = stats[i].value + statsRawPerLevel[i].value * (level.curLevel - 1);
        }
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct ApplyBuffJob : IJobEntity {
        [BurstCompile]
        public void Execute(
            in  DynamicBuffer<BuffBuffer>  buffs
          , ref DynamicBuffer<StatsBuffer> stats) {
            for (int i = 0; i < EnumCount.Stats; ++i) stats[i] = buffs[i].ApplyTo(stats[i]);
        }
    }
}