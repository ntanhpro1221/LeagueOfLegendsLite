using Unity.Entities;

namespace NGDtuanh.Entities.StateMachine {
    /// <summary>
    /// Run state update logic.<br/>
    /// (Run after the <see cref="StateEnterSystemGroup"/>)
    /// </summary>
    [UpdateInGroup(typeof(StateMachineSystemGroup))]
    [UpdateAfter(typeof(StateEnterSystemGroup))]
    public partial class StateUpdateSystemGroup : ComponentSystemGroup { }
}