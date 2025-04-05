using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct RemoveExpiredBuffsSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<EnumIndexData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var networkTime = SystemAPI.GetSingleton<NetworkTime>();
        if (!networkTime.IsFirstTimeFullyPredictingTick) return;

        var     curTick = networkTime.ServerTick;
        ref var statsId = ref SystemAPI.GetSingleton<EnumIndexData>().StatsType;

        foreach (var (
                buff
              , upcomingExpiringBuff)
            in SystemAPI
                .Query<
                    DynamicBuffer<BuffBuffer>
                  , DynamicBuffer<UpcomingExpiringBuffBuffer>>()
                .WithAll<Simulate>())
            while (upcomingExpiringBuff.Length > 0) {
                var expiredBuff = upcomingExpiringBuff[^1];

                // Check if the buff has expired
                if (!expiredBuff.IsExpired(curTick)) break;

                // Remove the buff value
                buff.ElementAt(statsId[expiredBuff.statsType]).Remove(
                    expiredBuff.applyType
                  , expiredBuff.value);

                // Remove the buff from the expired list
                upcomingExpiringBuff.RemoveAt(upcomingExpiringBuff.Length - 1);
            }
    }
}