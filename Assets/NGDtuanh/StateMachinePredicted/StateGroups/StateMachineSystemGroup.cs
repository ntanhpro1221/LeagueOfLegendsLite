using Unity.Entities;
using Unity.NetCode;

namespace NGDtuanh.Entities.StateMachine {
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    internal partial class StateMachineSystemGroup : ComponentSystemGroup { }
}