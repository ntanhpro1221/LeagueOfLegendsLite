using Unity.Entities;

namespace NGDtuanh.Entities.StateMachine {
    /// <summary>
    /// 1. Decide whether to exit state or not.<br/>
    /// 2. If we need to exit state, you will continue to do the following step (otherwise just stop).<br/>
    /// 3. Perform exit logic, disable state's tag.<br/>
    /// 4. Enable next state's tag and enable <see cref="StateNeedEnterTag"/>.<br/>
    /// </summary>
    [UpdateInGroup(typeof(StateMachineSystemGroup))]
    public partial class StateExitSystemGroup : ComponentSystemGroup { }
}
