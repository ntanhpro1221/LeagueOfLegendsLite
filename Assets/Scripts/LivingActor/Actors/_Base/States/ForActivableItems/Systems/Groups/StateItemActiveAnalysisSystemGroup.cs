using NGDtuanh.Entities.StateMachine;
using Unity.Entities;

[UpdateInGroup(typeof(StateMachineSystemGroup))]
[UpdateAfter(typeof(StateExitSystemGroup))]
[UpdateBefore(typeof(StateEnterSystemGroup))]
public partial class StateItemActiveAnalysisSystemGroup : ComponentSystemGroup { }