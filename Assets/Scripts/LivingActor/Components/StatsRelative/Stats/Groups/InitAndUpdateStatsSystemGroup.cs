using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderFirst = true)]
[UpdateAfter(typeof(CopyCommandBufferToInputSystemGroup))]
public partial class InitAndUpdateStatsSystemGroup : ComponentSystemGroup { }