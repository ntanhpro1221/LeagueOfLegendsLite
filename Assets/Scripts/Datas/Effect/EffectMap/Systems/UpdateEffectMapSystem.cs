using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(HandleEffectSystemGroup), OrderFirst = true)]
[UpdateAfter(typeof(HandleEffectIOSystem))]
public partial struct UpdateEffectMapSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (var (
            buffer
          , map
            ) in SystemAPI
            .Query<
                DynamicBuffer<EffectBuffer>
              , RefRW<EffectMap>
            >().WithAll<
                Simulate
            >())
            map.ValueRW.Update(buffer);
    }
}