using Unity.Entities;
using Unity.NetCode;

namespace NGDtuanh.Entities.StateMachine {
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial class StateMachineSystemGroup : ComponentSystemGroup { }
}
