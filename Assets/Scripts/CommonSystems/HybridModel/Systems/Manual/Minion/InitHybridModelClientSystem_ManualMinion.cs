using Unity.Burst;
using Unity.Entities;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct InitHybridModelClientSystem_ManualMinion : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<BattleInitData>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<
                HybridModelInitRequest
              , ManualPoolingHybridModel>()
            .Build());
    }

    public void OnUpdate(ref SystemState state) {
        var myTeam = SystemAPI.GetSingleton<BattleInitData>().teamType;
        var ecb = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (
                hybridData
              , teamData
              , minionTag
              , requestTrigger
              , hybridTrigger
              , entity)
            in SystemAPI
                .Query<
                    RefRW<HybridModelData>
                  , RefRO<TeamTypeData>
                  , RefRO<MinionTag>
                  , EnabledRefRW<HybridModelInitRequest>
                  , EnabledRefRW<HybridModelData>>()
                .WithPresent<HybridModelData>()
                .WithAll<ManualPoolingHybridModel>()
                .WithEntityAccess())
            InitHybridModelClientSystem.InitHybridData(
                PoolCenter.Instance.Minion.Instantiate(new(teamData.ValueRO.teamType, minionTag.ValueRO.id))
              , sameTeamWithMe: teamData.ValueRO.teamType == myTeam
              , isMyChamp: false
              , ref hybridData.ValueRW
              , requestTrigger
              , hybridTrigger
              , entity
              , ref ecb);
    }
}