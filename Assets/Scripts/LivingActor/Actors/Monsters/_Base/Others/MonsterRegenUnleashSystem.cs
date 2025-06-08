using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

/// <summary>
/// Regenerate health when <see cref="MonsterLeashDisabling"/>> is on. <br/>
/// </summary>
[UpdateInGroup(typeof(Between_CopyCommand_PredictedFixed_SystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct MonsterRegenUnleashSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<MonsterCommonBehaviourConfigData>();

        state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<
                Simulate
              , MonsterTag
              , MonsterLeashDisabling>()
            .Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var configData = SystemAPI.GetSingleton<MonsterCommonBehaviourConfigData>();
        var curTick    = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

        state.Dependency = new Job {
            curTick          = curTick
          , newNextRegenTick = curTick.WithBonusTick(configData.hpRegenUnleash_IntervalTick)
          , regenPercent     = configData.hpRegenUnleash_Percent
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(MonsterTag))]
    [WithPresent(typeof(MonsterLeashDisabling))]
    [WithDisabled(
        typeof(MonsterLeashAnchor)
      , typeof(MonsterDisableHealthRegen))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public NetworkTick curTick;
        public NetworkTick newNextRegenTick;
        public float       regenPercent;

        [BurstCompile]
        public void Execute(
            ref MonsterLeashDisabling      unleashData
          , ref HealthData                 healthData
          , in  DynamicBuffer<StatsBuffer> stats) {
            if (unleashData.nextRegenTick.IsNewerThan(curTick)) return;

            unleashData.nextRegenTick = newNextRegenTick;
            float maxHP = stats[StatsId.Health].value;
            healthData.value = math.min(maxHP
              , healthData.value + (maxHP * regenPercent / 100f)
            ).Quantizate3();
        }
    }
}