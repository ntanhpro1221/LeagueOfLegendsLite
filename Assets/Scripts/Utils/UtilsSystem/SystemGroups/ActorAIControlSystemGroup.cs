using NGDtuanh.Entities.StateMachine;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[UpdateBefore(typeof(StateMachineSystemGroup))]
public partial class ActorAIControlSystemGroup : ComponentSystemGroup { }