using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct InitHybridModelClientSystem : ISystem {
    private static readonly Color AllyHighlightColor  = Color.blue;
    private static readonly Color EnemyHighlightColor = Color.red;

    [ReadOnly] private ComponentLookup<DummyTag> dummyLookup;
    [ReadOnly] private ComponentLookup<GhostOwnerIsLocal> ghostOwnerLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<BattleInitData>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<HybridModelInitRequest>()
            .WithNone<ManualPoolingHybridModel>()
            .Build());

        dummyLookup = SystemAPI.GetComponentLookup<DummyTag>(
            isReadOnly: true);
        ghostOwnerLookup = SystemAPI.GetComponentLookup<GhostOwnerIsLocal>(
            isReadOnly: true);
    }

    public void OnUpdate(ref SystemState state) {
        dummyLookup.Update(ref state);
        ghostOwnerLookup.Update(ref state);

        var myTeam = SystemAPI.GetSingleton<BattleInitData>().teamType;
        var ecb = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (
                requestData
              , hybridData
              , teamData
              , requestTrigger
              , hybridTrigger
              , entity)
            in SystemAPI
                .Query<
                    RefRW<HybridModelInitRequest>
                  , RefRW<HybridModelData>
                  , RefRO<TeamTypeData>
                  , EnabledRefRW<HybridModelInitRequest>
                  , EnabledRefRW<HybridModelData>>()
                .WithPresent<HybridModelData>()
                .WithNone<ManualPoolingHybridModel>()
                .WithEntityAccess())
            InitHybridData(
                Object.Instantiate(requestData.ValueRO.prefabRef.Value)
              , sameTeamWithMe: teamData.ValueRO.teamType == myTeam
              , isMyChamp:
                ghostOwnerLookup.HasComponent(entity)
             && ghostOwnerLookup.IsComponentEnabled(entity)
             && !dummyLookup.HasComponent(entity)
              , ref hybridData.ValueRW
              , requestTrigger
              , hybridTrigger
              , entity
              , ref ecb);
    }

    public static void InitHybridData(
        GameObject                               model
      , bool                                     sameTeamWithMe
      , bool                                     isMyChamp
      , ref HybridModelData                      hybridData
      , in  EnabledRefRW<HybridModelInitRequest> requestTrigger
      , in  EnabledRefRW<HybridModelData>        hybridTrigger
      , in  Entity                               entity
      , ref EntityCommandBuffer                  ecb) {

        // Link model with cleanup data
        ecb.AddComponent(entity, new HybridModelCleanupData {
            objectRef = model
        });

        // Link model with HybridModelData
        hybridData = new HybridModelData {
            transformRef    = model.transform
          , animCtrlRef     = model.GetComponentInChildren<SharedAnimController>()
          , outlineRef      = model.GetComponentInChildren<Outline>()
          , skillPreviewRef = model.GetComponentInChildren<SkillPreviewShower>()
        };
        hybridData.outlineRef.Value.OutlineColor = sameTeamWithMe
            ? AllyHighlightColor
            : EnemyHighlightColor;

        // Set render queue of my champion
        if (isMyChamp)
            foreach (var renderer in hybridData.outlineRef.Value.GetComponentsInChildren<Renderer>())
            foreach (var material in renderer.materials)
                material.renderQueue = RenderQueueHelper.OwnChamp;

        // Mark init done
        hybridTrigger.ValueRW  = true;
        requestTrigger.ValueRW = false;
    }
}