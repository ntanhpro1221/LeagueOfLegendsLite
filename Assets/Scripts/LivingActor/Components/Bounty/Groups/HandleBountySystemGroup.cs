using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
[UpdateBefore(typeof(HandleInOut_Damage_Exp_Gold_SystemGroup))]
public partial class HandleBountySystemGroup : ComponentSystemGroup { }