using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct InitHybridHealthBarClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BattleClientData>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<HybridHealthBarInitRequest>();
    }

    public void OnUpdate(ref SystemState state) {
        if (BattleSceneLife.Instance == null) return;
        
        var ecb = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        TeamType localTeam = SystemAPI.GetSingleton<BattleClientData>().teamType;

        foreach (var (
            data
          , entity) in SystemAPI
            .Query<UpdateAspect>()
            .WithEntityAccess()) {
            var hybridData = new HybridHealthBarData();

            hybridData.dynamic.Init(data.SpawnRequest.ValueRO);

            if (data.NeedSpawnStickyBar)
                hybridData.sticky.Init(
                    data.TeamData.ValueRO.team == localTeam
                  , data.ChampTag.ValueRO.id);

            if (SystemAPI.HasComponent<HybridHealthBarData>(entity))
                ecb.SetComponent(entity, hybridData);
            else ecb.AddComponent(entity, hybridData);

            // remove need spawn tag 
            ecb.RemoveComponent<HybridHealthBarInitRequest>(entity);
        }
    }

    private readonly partial struct UpdateAspect : IAspect {
        public readonly RefRO<HybridHealthBarInitRequest> SpawnRequest;
        public readonly RefRO<TeamTypeData>               TeamData;

        [Optional] public readonly RefRO<ChampionTag> ChampTag;
        [Optional] public readonly RefRO<DummyTag>    DummyTag;

        public bool NeedSpawnStickyBar =>
            ChampTag.IsValid
         && !DummyTag.IsValid;
    }
}