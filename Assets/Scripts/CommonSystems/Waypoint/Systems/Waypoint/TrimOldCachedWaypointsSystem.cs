using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(UpdateWaypointSystemGroup))]
[UpdateAfter(typeof(ReturnWaypointRequestResultAndTrimSystem))]
public partial struct TrimOldCachedWaypointsSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<CachedPathData>();
        state.RequireForUpdate<CachedLineCastData>();
    }

    public void OnUpdate(ref SystemState state) {
        SystemAPI.ManagedAPI.GetSingleton<CachedPathData>().TrimOldData();
        SystemAPI.ManagedAPI.GetSingleton<CachedLineCastData>().TrimOldData();
    }
}