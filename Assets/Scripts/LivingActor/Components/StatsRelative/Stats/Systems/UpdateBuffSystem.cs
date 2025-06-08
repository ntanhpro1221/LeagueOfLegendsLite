using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(HandleStatsSystemGroup))]
[UpdateBefore(typeof(InitAndUpdateStatsSystem))]
public partial struct UpdateBuffSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job {
            curTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public NetworkTick curTick;

        [BurstCompile]
        public void Execute(
            ref DynamicBuffer<BuffBuffer>                 buffs
          , ref DynamicBuffer<IncomingBuffBuffer>         incomingBuffs
          , ref DynamicBuffer<IncomingExpiringBuffBuffer> incomingExpiringBuffs) {
            // Add new buff
            while (!incomingBuffs.IsEmpty) {
                var newBuff = incomingBuffs.PopBack();
                buffs.ElementAt((int)newBuff.statsType).Add(newBuff.applyType, newBuff.value);
                if (newBuff.durationType == BuffDurationType.Temp)
                    incomingExpiringBuffs.InsertSorted(IncomingExpiringBuffBuffer.Construct(newBuff, curTick));
            }
            
            // Remove old buff
            while (!incomingExpiringBuffs.IsEmpty) {
                var oldestBuff = incomingExpiringBuffs.BackRO();
                if (!oldestBuff.IsExpired(curTick)) break;
                buffs.ElementAt((int)oldestBuff.statsType).Remove(oldestBuff.applyType, oldestBuff.value);
                incomingExpiringBuffs.PopBack();
            }
        }
    }
}