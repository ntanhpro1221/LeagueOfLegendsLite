using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderFirst = true)]
[UpdateAfter(typeof(CopyCommandBufferToInputSystemGroup))]
[UpdateBefore(typeof(PredictedFixedStepSimulationSystemGroup))]
public partial class Between_CopyCommand_PredictedFixed_SystemGroup : ComponentSystemGroup { }