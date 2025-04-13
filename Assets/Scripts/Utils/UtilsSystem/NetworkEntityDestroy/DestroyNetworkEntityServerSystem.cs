using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
[UpdateInGroup(typeof(DestroyNetworkEntitySystemGroup))]
public partial struct DestroyNetworkEntityServerSystem : ISystem {
    private EntityQuery _NetworkDestroyedQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate(_NetworkDestroyedQuery = SystemAPI.QueryBuilder()
            .WithAll<
                NetworkDestroyedTag
              , Simulate>()
            .Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick) return;

        state.EntityManager.DestroyEntity(_NetworkDestroyedQuery);
    }
}