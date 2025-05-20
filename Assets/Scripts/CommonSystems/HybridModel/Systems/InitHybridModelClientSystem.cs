using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct InitHybridModelClientSystem : ISystem {
    private static readonly Color AllyHighlightColor  = Color.blue;
    private static readonly Color EnemyHighlightColor = Color.red;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<BattleInitData>();
        state.RequireForUpdate<HybridModelInitRequest>();
    }

    public void OnUpdate(ref SystemState state) {
        var ecb = SystemAPI
            .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        var myTeam = SystemAPI.GetSingleton<BattleInitData>().teamType;

        foreach (var (
            spawnRequest
          , teamType
          , entity) in SystemAPI
            .Query<
                RefRO<HybridModelInitRequest>
              , RefRO<TeamTypeData>>()
            .WithEntityAccess()) {

            // spawn
            var model = Object.Instantiate(spawnRequest.ValueRO.prefabRef.Value);

            // Link model with HybridModelData
            var hybridData = new HybridModelData {
                transformRef    = model.transform
              , animCtrlRef     = model.GetComponentInChildren<SharedAnimController>()
              , outlineRef      = model.GetComponentInChildren<Outline>()
              , skillPreviewRef = model.GetComponentInChildren<SkillPreviewShower>()
              , rotateRef       = model.GetComponentInChildren<RotationController>()
            };
            hybridData.outlineRef.Value.OutlineColor = teamType.ValueRO.team == myTeam
                ? AllyHighlightColor
                : EnemyHighlightColor;

            // Set render queue of my champion
            if (SystemAPI.HasComponent<GhostOwnerIsLocal>(entity)
             && SystemAPI.IsComponentEnabled<GhostOwnerIsLocal>(entity))
                foreach (var renderer in hybridData.outlineRef.Value.GetComponentsInChildren<Renderer>())
                foreach (var material in renderer.materials)
                    material.renderQueue = RenderQueueHelper.OwnChamp;

            // Add/Set data
            if (SystemAPI.HasComponent<HybridModelData>(entity))
                ecb.SetComponent(entity, hybridData);
            else ecb.AddComponent(entity, hybridData);

            // remove need spawn tag
            ecb.RemoveComponent<HybridModelInitRequest>(entity);
        }
    }
}