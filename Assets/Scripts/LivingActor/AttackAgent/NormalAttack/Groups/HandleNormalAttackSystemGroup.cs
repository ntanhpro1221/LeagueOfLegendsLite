using NGDtuanh.Entities.StateMachine;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[UpdateAfter(typeof(StateMachineSystemGroup))]
public partial class HandleNormalAttackSystemGroup : ComponentSystemGroup { }