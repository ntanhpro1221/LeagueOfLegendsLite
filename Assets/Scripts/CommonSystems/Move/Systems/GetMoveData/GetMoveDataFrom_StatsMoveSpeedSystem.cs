using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(BeforeMoveSystemGroup))]
public partial struct GetMoveDataFrom_StatsMoveSpeedSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EnumIndexData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var moveSpeedId = SystemAPI.GetSingleton<EnumIndexData>().StatsType[StatsType.MoveSpeed];

        foreach (var (
                moveData
              , statsData)
            in SystemAPI.Query<
                    RefRW<MoveData>
                  , DynamicBuffer<StatsBuffer>>()
                .WithAll<Simulate>()
                .WithNone<NetworkDestroyedTag>())
            moveData.ValueRW.moveSpeed = statsData[moveSpeedId].value;
    }
}