using Unity.Entities;
using Unity.NetCode;

/// <summary>
/// - Just don't mark your system with <see cref="UpdateInGroupAttribute.OrderLast"/> because you will want it to run before <see cref="ClearIncomingDamageSystem"/>.<br/>
/// - And your also don't need worry about FirstTimeFullPredictingTick unless you spawn or destroy something
/// </summary>
[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
public partial class HandleIncomingDamageSystemGroup : ComponentSystemGroup { }