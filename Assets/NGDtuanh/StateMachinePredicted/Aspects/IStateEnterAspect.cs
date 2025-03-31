using Unity.Entities;

namespace NGDtuanh.Entities.StateMachine {
    public interface IStateEnterAspect<TIdentity, TState> : IStateAspect<TIdentity, TState>
        where TIdentity : unmanaged, IComponentData
        where TState : unmanaged, IComponentData, IEnableableComponent {
        protected EnabledRefRO<StateRequireEnter> StateRequireEnter { get; }
    }
}