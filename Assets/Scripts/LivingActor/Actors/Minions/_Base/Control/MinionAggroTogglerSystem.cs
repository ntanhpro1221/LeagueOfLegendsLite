using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ActorAIControlSystemGroup))]
public partial struct MinionAggroTogglerSystem : ISystem {
    [ReadOnly] private ComponentLookup<LocalTransform> locTransLookup;
    [ReadOnly] private BufferLookup<StatsBuffer>       statsLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EnumIndexData>();
        state.RequireForUpdate<ClientServerTickRate>();
        state.RequireForUpdate<MinionBehaviourConfigData>();
        state.RequireForUpdate<NetworkTime>();

        locTransLookup = SystemAPI.GetComponentLookup<LocalTransform>(
            isReadOnly: true);
        statsLookup = SystemAPI.GetBufferLookup<StatsBuffer>(
            isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        locTransLookup.Update(ref state);
        statsLookup.Update(ref state);

        var curTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
        var doneAtTick = curTick.WithDeltaTime(
            SystemAPI.GetSingleton<MinionBehaviourConfigData>().aggroCD
          , SystemAPI.GetSingleton<ClientServerTickRate>().SimulationTickRate);

        state.Dependency = new DisableJob {
            locTransLookup = locTransLookup
          , statsLookup    = statsLookup
          , doneAtTick     = doneAtTick
            , radiusId = SystemAPI.GetSingleton<EnumIndexData>().StatsType[StatsType.UnitRadius]
        }.ScheduleParallel(state.Dependency);

        state.Dependency = new EnableJob {
            curTick = curTick
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(MinionTag))]
    [WithPresent(
        typeof(AggroDisabling))]
    [BurstCompile]
    private partial struct DisableJob : IJobEntity {
        [ReadOnly] public ComponentLookup<LocalTransform> locTransLookup;
        [ReadOnly] public BufferLookup<StatsBuffer>       statsLookup;

        public NetworkTick doneAtTick;
        public int         radiusId;

        [BurstCompile]
        public void Execute(
            ref AggroAnchor                         aggroAnchorData
          , EnabledRefRW<AggroAnchor>               aggroAnchorEnable
          , ref AggroDisabling                      aggroDisableData
          , EnabledRefRW<AggroDisabling>            aggroDisableTrigger
          , in DynamicBuffer<MinionFixedPathBuffer> pathBuffer
          , in LocalTransform                       locTrans
          , in MinionControlFactor                  controlFactor
          , in AimedTargetData                      target) {
            if (controlFactor.aggroRangeSqr < GameHelpers.DistanceXZ_Sqr(locTrans.Position
                  , aggroAnchorData.anchor)
             || controlFactor.aggroRangeSqr < (
                    GameHelpers.DistanceXZ(locTrans.Position, locTransLookup[target.target].Position)
                  - statsLookup[target.target][radiusId].value).Sqr()) {

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
        public NetworkTick curTick;

        [BurstCompile]
        public void Execute(
            ref AggroDisabling                      disableData
          , EnabledRefRW<AggroDisabling>            disableTrigger
          , in DynamicBuffer<MinionFixedPathBuffer> pathBuffer) {
            if ( // Done cooldown
                curTick.IsNewerThan(disableData.doneAtTick)
                // Reach some path point
             || pathBuffer.Length < disableData.pathLengthWhenDiable)
                disableTrigger.ValueRW = false;
        }
    }
}