using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct InitHybridModelClientSystem : ISystem {
    private static readonly Color AllyHighlightColor = Color.blue;
    private static readonly Color EnemyHighlightColor = Color.red;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BattleInitData>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<HybridModelInitRequest>()
            .Build());
    }

    public void OnUpdate(ref SystemState state) {
        using EntityCommandBuffer ecb = new(Allocator.Temp);

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
                transformRef = model.transform
              , animCtrlRef  = model.GetComponent<SharedAnimController>()
              , outlineRef   = model.GetComponent<Outline>()
            };
            hybridData.outlineRef.Value.OutlineColor = teamType.ValueRO.teamType == myTeam
                ? AllyHighlightColor
                : EnemyHighlightColor;
            
            if (SystemAPI.HasComponent<HybridModelData>(entity))
                ecb.SetComponent(entity, hybridData);
            else ecb.AddComponent(entity, hybridData);

            // remove need spawn tag
            ecb.RemoveComponent<HybridModelInitRequest>(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}