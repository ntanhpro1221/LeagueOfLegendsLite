using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
[UpdateBefore(typeof(DestroyNetworkEntitySystemGroup))]
public partial class BeforeDestroyNetworkEntitySystemGroup : ComponentSystemGroup { }