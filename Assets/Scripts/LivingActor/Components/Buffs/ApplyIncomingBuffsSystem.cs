using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct ApplyIncomingBuffsSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<ClientServerTickRate>();
        state.RequireForUpdate<EnumIndexData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var networkTime = SystemAPI.GetSingleton<NetworkTime>();
        if (!networkTime.IsFirstTimeFullyPredictingTick) return;

        var     curTick  = networkTime.ServerTick;
        var     tickRate = SystemAPI.GetSingleton<ClientServerTickRate>().SimulationTickRate;
        ref var statsId  = ref SystemAPI.GetSingleton<EnumIndexData>().StatsType;

        foreach (var (
                buffs
              , incomingBuffs
              , upcomingExpiringBuffs)
            in SystemAPI
                .Query<
                    DynamicBuffer<BuffBuffer>
                  , DynamicBuffer<IncomingBuffBuffer>
                  , DynamicBuffer<UpcomingExpiringBuffBuffer>>()
                .WithAll<Simulate>()) {
            foreach (var newBuf in incomingBuffs) {
                buffs.ElementAt(statsId[newBuf.statsType]).Add(
                    newBuf.applyType
                  , newBuf.value);

                if (newBuf.durationType == BuffDurationType.Temp)
                    upcomingExpiringBuffs.Add(UpcomingExpiringBuffBuffer.Construct(newBuf, curTick, tickRate));
            }

            incomingBuffs.Clear();
        }
    }
}