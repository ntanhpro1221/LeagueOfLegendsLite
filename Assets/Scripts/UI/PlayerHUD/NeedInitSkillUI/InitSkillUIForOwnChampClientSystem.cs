using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial struct InitSkillUIForOwnChampClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<
                ChampionTag
              , NeedInitSkillUI
              , GhostOwnerIsLocal
            >().WithNone<
                DummyTag
            >().Build());
    }

    public void OnUpdate(ref SystemState state) {
        var ecb = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (
            tag
          , entity
            ) in SystemAPI
            .Query<
                ChampionTag
            >().WithAll<
                NeedInitSkillUI
              , GhostOwnerIsLocal
            >().WithNone<
                DummyTag
            >().WithEntityAccess()) {
            PlayerHUD.Instance.Skills.InitAll(tag.id);
            ecb.RemoveComponent<NeedInitSkillUI>(entity);
        }
    }
}