using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
public partial class DestroyNetworkEntitySystemGroup : ComponentSystemGroup { }