using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial class InputLocalUpdateSystemGroup : ComponentSystemGroup { }