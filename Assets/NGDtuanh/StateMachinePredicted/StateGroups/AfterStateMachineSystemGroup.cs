using Unity.Entities;

namespace NGDtuanh.Entities.StateMachine {
    [UpdateInGroup(typeof(StateMachineSystemGroup), OrderLast = true)]
    public partial class AfterStateMachineSystemGroup : ComponentSystemGroup { }
}