using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderFirst = true)]
[UpdateBefore(typeof(PredictedFixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(CopyCommandBufferToInputSystemGroup))]
public partial class MoveSystemGroup : ComponentSystemGroup { }