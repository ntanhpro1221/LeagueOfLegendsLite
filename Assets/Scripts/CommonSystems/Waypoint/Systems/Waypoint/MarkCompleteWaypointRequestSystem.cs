using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(UpdateWaypointSystemGroup))]
[UpdateAfter(typeof(HandleWaypointRequestSystem))]
public partial struct MarkCompleteWaypointRequestSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (var trigger in SystemAPI
            .Query<EnabledRefRW<NeedHandleWaypointRequest>>()
            .WithAll<Simulate>())
            // Mark request completed
            trigger.ValueRW = false;
    }
}