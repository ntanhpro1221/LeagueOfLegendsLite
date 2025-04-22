using Unity.Entities;

[UpdateInGroup(typeof(Between_CopyCommand_PredictedFixed_SystemGroup))]
[UpdateAfter(typeof(InputPredictedUpdateSystemGroup))]
public partial class PrepareMoveSystemGroup : ComponentSystemGroup { }