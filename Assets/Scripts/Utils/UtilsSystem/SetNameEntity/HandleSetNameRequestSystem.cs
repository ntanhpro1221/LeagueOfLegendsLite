using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

public partial struct HandleSetNameRequestSystem : ISystem {
    private EntityQuery mainQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        mainQuery = SystemAPI.QueryBuilder()
            .WithAll<
                SetNameRequest
            >().Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (mainQuery.IsEmpty) return;

        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (
            request
          , requestTrigger
          , entity
            ) in SystemAPI
            .Query<
                RefRW<SetNameRequest>
              , EnabledRefRW<SetNameRequest>
            >().WithEntityAccess()) {
            ecb.SetName(entity, request.ValueRO.name);
            requestTrigger.ValueRW = false;
        }

        ecb.Playback(state.EntityManager);
    }
}