using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct InitHybridModelClientSystem : ISystem {
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
            data
          , entity
            ) in SystemAPI
            .Query<UpdateAspect>()
            .WithEntityAccess()) {
            var hybridData = new HybridModelData();

            hybridData.InitRealModel(data, data.TeamData.ValueRO.team == myTeam);

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

    public readonly partial struct UpdateAspect : IAspect {
        public readonly RefRO<HybridModelInitRequest> SpawnRequest;
        public readonly RefRO<TeamTypeData>           TeamData;

        [Optional] public readonly RefRO<ChampionTag> ChampTag;
    }
}