using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(UpdateWaypointSystemGroup))]
public partial struct FixWaypointSystem : ISystem {
    public void OnUpdate(ref SystemState state) {
        foreach (var (
                locTrans
              , waypoints
              , requestNewWaypoint)
            in SystemAPI
                .Query<
                    RefRO<LocalTransform>
                  , DynamicBuffer<WaypointBuffer>
                  , EnabledRefRW<NeedHandleWaypointRequest>>()
                .WithAll<Simulate>()) {
            if (waypoints.Empty()) continue;

            if (!AstarPath.active.Linecast(locTrans.ValueRO.Position, waypoints.FrontRO().pos)) continue;

            requestNewWaypoint.ValueRW = true;
        }
    }
}