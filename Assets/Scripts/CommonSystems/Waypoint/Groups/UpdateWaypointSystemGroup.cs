using Unity.Entities;

[UpdateInGroup(typeof(WaypointSystemGroup))]
[UpdateAfter(typeof(UpdateGraphSystemGroup))]
public partial class UpdateWaypointSystemGroup : ComponentSystemGroup { }