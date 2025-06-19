using Unity.Entities;

namespace NGDtuanh.Entities.StateMachine {
    [UpdateInGroup(typeof(StateMachineSystemGroup), OrderFirst = true)]
    public partial class BeforeStateMachineSystemGroup : ComponentSystemGroup { }
}