using Unity.Entities;

[UpdateInGroup(typeof(WaypointSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation, WorldSystemFilterFlags.ServerSimulation)]
[UpdateBefore(typeof(UpdateWaypointSystemGroup))]
public partial class UpdateObstacleSystemGroup : ComponentSystemGroup { }