using Unity.Entities;

[UpdateInGroup(typeof(Between_CopyCommand_PredictedFixed_SystemGroup), OrderFirst = true)]
[UpdateBefore(typeof(DestroyNetworkEntitySystemGroup))]
public partial class BeforeDestroyNetworkEntitySystemGroup : ComponentSystemGroup { }