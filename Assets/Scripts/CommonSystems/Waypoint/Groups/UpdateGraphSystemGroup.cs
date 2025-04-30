using Unity.Entities;

[UpdateInGroup(typeof(WaypointSystemGroup))]
[UpdateAfter(typeof(UpdateObstacleSystemGroup))]
public partial class UpdateGraphSystemGroup : ComponentSystemGroup { }