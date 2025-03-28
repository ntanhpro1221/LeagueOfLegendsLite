using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// delete all targets in <see cref="DeleteObjectData"/> include itself if there is a <see cref="DeleteObjectRequest"/>
/// </summary>
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct DeleteObjectSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<DeleteObjectRequest>();
    }

    public void OnUpdate(ref SystemState state) {
        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (request, entity) in
            SystemAPI.Query<RefRO<DeleteObjectRequest>>()
                .WithEntityAccess()) {
            var data = DeleteObjectData.Instance;
            if (data != null) {
                if (data.targets != null)
                    foreach (var item in data.targets)
                        Object.Destroy(item);

                // data will be destroyed here for performance reason.
                // Then you just have one time to send request foreach scene you place this singleton.
                Object.Destroy(data.gameObject);
            }

            if (request.ValueRO.deleteRequestEntity) ecb.DestroyEntity(entity);
            else ecb.RemoveComponent<DeleteObjectRequest>(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}