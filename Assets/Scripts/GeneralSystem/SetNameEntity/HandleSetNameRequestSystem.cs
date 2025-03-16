using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

public partial struct HandleSetNameRequestSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<SetNameRequest>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (setNameRequest, entity) in SystemAPI.Query<RefRO<SetNameRequest>>().WithEntityAccess()) {
            ecb.SetName(entity, setNameRequest.ValueRO.name);
            ecb.RemoveComponent<SetNameRequest>(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}