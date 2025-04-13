using Unity.Entities;

[UpdateInGroup(typeof(Between_CopyCommand_PredictedFixed_SystemGroup), OrderFirst = true)]
public partial class DestroyNetworkEntitySystemGroup : ComponentSystemGroup { }