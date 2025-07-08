using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
[UpdateInGroup(typeof(TeamMemberHandleSystemGroup))]
public partial struct UpdateBanPickUI_ClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkId>();
    }

    public void OnUpdate(ref SystemState state) {
        if (!BanPickMenuUI.IsAvailable) return;
        
        foreach (var (
            buffer
          , data
            ) in SystemAPI
            .Query<
                DynamicBuffer<TeamMemberBuffer>
              , RefRO<TeamMemberData>
            >())
            if (data.ValueRO.NeedUpdateUI) {
                BanPickMenuUI.Instance.ForceUpdateTeamListUI(
                    buffer
                  , SystemAPI.GetSingleton<NetworkId>());

                bool isAllPlayerDonePickChamp = true;
                foreach (var member in buffer)
                    if (!member.lockedChamp)
                        isAllPlayerDonePickChamp = false;
                BanPickMenuUI.Instance.StartGameBtn.UpdateState(
                    isHost: BanPickBootstrapper.Instance.IsHost
                  , isAllPlayerDonePickChamp: isAllPlayerDonePickChamp);
            }
    }
}