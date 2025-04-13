using Unity.Entities;

[UpdateInGroup(typeof(Between_CopyCommand_PredictedFixed_SystemGroup))]
[UpdateAfter(typeof(InputPredictedUpdateSystemGroup))]
[UpdateBefore(typeof(MoveSystemGroup))]
public partial class BeforeMoveSystemGroup : ComponentSystemGroup { }