using Unity.Entities;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(BeforePhysicsSystemGroup))]
public partial class MoveSystemGroup : ComponentSystemGroup { }