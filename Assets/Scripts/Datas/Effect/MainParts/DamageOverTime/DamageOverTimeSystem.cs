using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(HandleEffectSystemGroup))]
public partial struct DamageOverTimeSystem : ISystem {
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

    [WithAll(
        typeof(Simulate))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public NetworkTick curTick;

        [BurstCompile]
        public void Execute(
            ref DynamicBuffer<EffectBuffer>         effects
          , ref DynamicBuffer<IncomingDamageBuffer> damageBuffer) {
            // Perform all damage over time
            for (int i = 0; i < effects.Length; ++i) {
                ref var damageOT = ref effects.ElementAt(i).damageOT;

                if ( // Not use
                    !damageOT.enable
                    // Not ready to deal damage
                 || !damageOT.ItsTimeToDamage(curTick)) continue;

                // Update next damage tick
                damageOT.UpdateNextDamageTick(curTick);

                // Perform damage
                damageBuffer.Add(new IncomingDamageBuffer {
                    damage = damageOT.damage
                  , source = effects[i].id.source
                });
            }
        }
    }
}