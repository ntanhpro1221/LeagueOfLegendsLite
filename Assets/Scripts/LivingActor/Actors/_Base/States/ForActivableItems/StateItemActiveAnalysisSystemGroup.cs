using NGDtuanh.Entities.StateMachine;
using Unity.Entities;

/// <summary>
/// Just dont set <see cref="UpdateInGroupAttribute.OrderLast"/> flags because this place is for <see cref="AutoExitItemActiveAnalyzingSystem"/>
/// </summary>
[UpdateInGroup(typeof(StateMachineSystemGroup))]
[UpdateAfter(typeof(StateExitSystemGroup))]
[UpdateBefore(typeof(StateEnterSystemGroup))]
public partial class StateItemActiveAnalysisSystemGroup : ComponentSystemGroup { }