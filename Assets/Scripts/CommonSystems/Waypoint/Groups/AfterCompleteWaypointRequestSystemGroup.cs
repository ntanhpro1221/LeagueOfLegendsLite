using Unity.Entities;

[UpdateInGroup(typeof(UpdateWaypointSystemGroup))]
[UpdateAfter(typeof(HandleWaypointRequestSystem))]
[UpdateBefore(typeof(MarkCompleteWaypointRequestSystem))]
public partial class AfterCompleteWaypointRequestSystemGroup : ComponentSystemGroup { }