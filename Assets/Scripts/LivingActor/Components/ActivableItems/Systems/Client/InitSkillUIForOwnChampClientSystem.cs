using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial struct InitSkillUIForOwnChampClientSystem : ISystem {
    private EntityQuery mainQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();

        mainQuery = SystemAPI.QueryBuilder()
            .WithAll<
                ChampionTag
              , NeedInitSkillUI
              , GhostOwnerIsLocal
            >().WithNone<
                DummyTag
            >().Build();
    }

    public void OnUpdate(ref SystemState state) {
        if (mainQuery.IsEmpty) return;
        
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
            PlayerHUD.Instance.ActivableItems.InitAllSkills(tag.id);
            ecb.RemoveComponent<NeedInitSkillUI>(entity);
        }
    }
}