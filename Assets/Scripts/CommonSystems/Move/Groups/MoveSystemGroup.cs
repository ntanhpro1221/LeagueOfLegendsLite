using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(Between_CopyCommand_PredictedFixed_SystemGroup))]
[UpdateAfter(typeof(InputPredictedUpdateSystemGroup))]
public partial class MoveSystemGroup : ComponentSystemGroup { }