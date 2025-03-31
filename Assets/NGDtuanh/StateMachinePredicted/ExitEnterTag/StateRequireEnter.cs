using Unity.Entities;

namespace NGDtuanh.Entities.StateMachine {
    /// <summary>
    /// Indicate that this state is waiting for performing enter logic.<br/>
    /// </summary>
    public struct StateRequireEnter : IComponentData, IEnableableComponent { }
}