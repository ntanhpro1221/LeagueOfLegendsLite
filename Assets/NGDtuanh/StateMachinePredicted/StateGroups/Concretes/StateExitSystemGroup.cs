using Unity.Entities;

namespace NGDtuanh.Entities.StateMachine {
    /// <summary>
    /// - Query for <see cref="StateNotExitedYet"/> and try to run state exit logic.<br/>
    /// - If exit logic has been executed successfully, <b>MARK THE STATE AS EXITED</b>.<br/>
    /// </summary>
    [UpdateInGroup(typeof(StateMachineSystemGroup))]
    public partial class StateExitSystemGroup : ComponentSystemGroup { }
}