using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ActorAIControlSystemGroup))]
[UpdateBefore(typeof(MinionControlSystem))]
public partial struct MinionAggroTogglerSystem : ISystem {
    [ReadOnly] private ComponentLookup<LocalTransform> locTransLookup;
    [ReadOnly] private ComponentLookup<ChampionTag>    champLookup;
    [ReadOnly] private ComponentLookup<StatsData>      statsLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        Debug.LogWarning("NGDtuanh TEST: tmp disable aggro cooldown time");
        state.RequireForUpdate<ClientServerTickRate>();
        state.RequireForUpdate<MinionCommonBehaviourConfigData>();
        state.RequireForUpdate<NetworkTime>();

        locTransLookup = SystemAPI.GetComponentLookup<LocalTransform>(
            isReadOnly: true);
        champLookup = SystemAPI.GetComponentLookup<ChampionTag>(
            isReadOnly: true);
        statsLookup = SystemAPI.GetComponentLookup<StatsData>(
            isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        locTransLookup.Update(ref state);
        champLookup.Update(ref state);
        statsLookup.Update(ref state);

        var curTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
        var doneAtTick = curTick.WithDeltaTime(
            SystemAPI.GetSingleton<MinionCommonBehaviourConfigData>().aggroCD
          , SystemAPI.GetSingleton<ClientServerTickRate>().SimulationTickRate);

        state.Dependency = new DisableJob {
            locTransLookup = locTransLookup
          , statsLookup    = statsLookup
          , doneAtTick     = doneAtTick
        }.ScheduleParallel(state.Dependency);

        state.Dependency = new EnableJob {
            champLookup = champLookup
          , curTick     = curTick
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(MinionTag))]
    [WithPresent(
        typeof(MinionAggroDisabling))]
    [BurstCompile]
    private partial struct DisableJob : IJobEntity {
        [ReadOnly] public ComponentLookup<LocalTransform> locTransLookup;
        [ReadOnly] public ComponentLookup<StatsData>      statsLookup;

        public NetworkTick doneAtTick;

        [BurstCompile]
        public void Execute(
            ref MinionAggroAnchor                   aggroAnchorData
          , EnabledRefRW<MinionAggroAnchor>         aggroAnchorEnable
          , ref MinionAggroDisabling                aggroDisableData
          , EnabledRefRW<MinionAggroDisabling>      aggroDisableTrigger
          , in DynamicBuffer<MinionFixedPathBuffer> pathBuffer
          , in LocalTransform                       locTrans
          , in MinionControlFactor                  controlFactor
          , in AimedTargetData                      target) {
            if (controlFactor.aggroRangeSqr < GameHelpers.DistanceXZ_Sqr(locTrans.Position
                  , aggroAnchorData.anchor)
             || controlFactor.aggroRangeSqr < (
                    GameHelpers.DistanceXZ(locTrans.Position, locTransLookup[target.target].Position)
                  - statsLookup[target.target].data.UnitRadius).Sqr()) {

                aggroDisableData.doneAtTick           = doneAtTick;
                aggroDisableData.pathLengthWhenDiable = pathBuffer.Length;

                aggroAnchorEnable.ValueRW   = false;
                aggroDisableTrigger.ValueRW = true;
            }
        }
    }

    [WithAll(
        typeof(Simulate)
      , typeof(MinionTag))]
    [BurstCompile]
    private partial struct EnableJob : IJobEntity {
        [ReadOnly] public ComponentLookup<ChampionTag> champLookup;

        public NetworkTick curTick;

        [BurstCompile]
        public void Execute(
            ref MinionAggroDisabling                disableData
          , EnabledRefRW<MinionAggroDisabling>      disableTrigger
          , in DynamicBuffer<MinionFixedPathBuffer> pathBuffer
          , in AimedTargetData                      target) {
            if (
                // Done cooldown
                // curTick.IsNewerThan(disableData.doneAtTick) ||

                // Have target but target is not champion (turret or another minion)
                champLookup.EntityExists(target.target) && !champLookup.HasComponent(target.target)
                // Reach some path point
             || pathBuffer.Length < disableData.pathLengthWhenDiable)
                disableTrigger.ValueRW = false;
        }
    }
}