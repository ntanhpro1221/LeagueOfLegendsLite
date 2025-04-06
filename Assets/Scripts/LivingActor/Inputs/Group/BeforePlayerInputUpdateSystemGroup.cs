using Unity.Entities;

[UpdateInGroup(typeof(PlayerInputUpdateSystemGroup))]
[UpdateBefore(typeof(PlayerInputUpdateSystem))]
public partial class BeforePlayerInputUpdateSystemGroup : ComponentSystemGroup { }