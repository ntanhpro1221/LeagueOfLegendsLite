using Unity.Entities;

namespace NGDtuanh.Entities.StateMachine {
    /// <summary>
    /// Query for <see cref="StateRequireEnter"/> and perform state enter logic.<br/>
    /// (Run after <see cref="StateExitSystemGroup"/>)<br/>
    /// </summary>
    [UpdateInGroup(typeof(StateMachineSystemGroup))]
    [UpdateAfter(typeof(StateExitSystemGroup))]
    public partial class StateEnterSystemGroup : ComponentSystemGroup { }
}