using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct ApplyIncomingExpSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<RequireExpData>();
        state.RequireForUpdate<NetworkTime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick) return;

        var requireExp = SystemAPI.GetSingleton<RequireExpData>();

        foreach (var (
                levelData
              , incomingExp)
            in SystemAPI
                .Query<
                    RefRW<LevelData>
                  , DynamicBuffer<IncomingExpBuffer>>()
                .WithAll<Simulate>()) {
            // add exp
            foreach (var exp in incomingExp)
                levelData.ValueRW.curExp += exp.exp;
            incomingExp.Clear();

            // level up
            while (levelData.ValueRO.curLevel < requireExp.MaxLevel) {
                int nextLevelExp = requireExp.CalcRequireExpForNextLevel(levelData.ValueRO.curLevel);
                if (levelData.ValueRO.curExp < nextLevelExp) break;
                levelData.ValueRW.curExp -= nextLevelExp;
                levelData.ValueRW.curLevel++;
            }
        }
    }
}