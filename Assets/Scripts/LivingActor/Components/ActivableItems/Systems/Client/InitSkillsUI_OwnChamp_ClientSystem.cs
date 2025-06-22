using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(HandleItemClientUISystemGroup))]
public partial struct InitSkillsUI_OwnChamp_ClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
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
                RefRO<ChampionTag>
            >().WithAll<
                NeedInitSkillUI
              , GhostOwnerIsLocal
            >().WithNone<
                DummyTag
            >().WithEntityAccess()) {
            PlayerHUD.Instance.ActivableItems.InitAllSkills(tag.ValueRO.id);
            ecb.RemoveComponent<NeedInitSkillUI>(entity);
        }
    }
}