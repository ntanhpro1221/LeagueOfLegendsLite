using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
[UpdateBefore(typeof(HandleIncomingDamageSystemGroup))]
public partial class HandleBountySystemGroup : ComponentSystemGroup { }