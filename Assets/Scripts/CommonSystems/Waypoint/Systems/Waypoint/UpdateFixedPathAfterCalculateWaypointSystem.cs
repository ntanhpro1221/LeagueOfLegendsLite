using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(AfterCompleteWaypointRequestSystemGroup))]
public partial struct UpdateFixedPathAfterCalculateWaypointSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (var (
            pathBuffer
          , waypointBuffer) in SystemAPI
            .Query<
                DynamicBuffer<MinionFixedPathBuffer>
              , DynamicBuffer<WaypointBuffer>>()
            .WithAll<
                Simulate
              , NeedHandleWaypointRequest>()
            .WithNone<AutoFollowTarget>())
            pathBuffer.ElementAt(0).pos = waypointBuffer[0].pos;
    } 
}
