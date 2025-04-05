using Unity.Entities;

[UpdateInGroup(typeof(MoveSystemGroup))]
[UpdateBefore(typeof(ApplyMoveSystem))]
public partial class BeforeMoveSystemGroup : ComponentSystemGroup { }