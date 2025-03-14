using Unity.Entities;
using Unity.NetCode;

namespace NGDtuanh.Entities.StateMachine {
    /// <summary>
    /// Run state fixed update logic.
    /// </summary>
    [UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup))]
    public partial class StateFixedUpdateSystemGroup : ComponentSystemGroup { }
}