using Unity.Entities;

[UpdateInGroup(typeof(InitAndUpdateStatsSystemGroup))]
[UpdateBefore(typeof(InitAndUpdateStatsSystem))]
public partial class BeforeInitAndUpdateStatsSystemGroup : ComponentSystemGroup { }