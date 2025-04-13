using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
[UpdateBefore(typeof(InputLocalUpdateSystemGroup))]
public partial class BeforeInputLocalUpdateSystemGroup : ComponentSystemGroup { }