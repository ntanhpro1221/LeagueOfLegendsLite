using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
public partial class HandleInOut_Damage_Exp_Gold_SystemGroup : ComponentSystemGroup { }