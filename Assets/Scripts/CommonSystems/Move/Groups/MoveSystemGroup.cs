using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderFirst = true)]
[UpdateBefore(typeof(PredictedFixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(CopyCommandBufferToInputSystemGroup))]
// [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial class MoveSystemGroup : ComponentSystemGroup { }