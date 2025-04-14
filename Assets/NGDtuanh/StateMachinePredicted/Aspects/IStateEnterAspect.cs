using Unity.Entities;

namespace NGDtuanh.Entities.StateMachine {
    public interface IStateEnterAspect<TIdentity, TState> : IStateAspect<TIdentity, TState>
        where TIdentity : unmanaged, IComponentData
        where TState : unmanaged, IComponentData, IEnableableComponent {
        protected EnabledRefRO<StateRequireEnter> StateRequireEnter { get; }

        // ReSharper disable once PossibleInterfaceMemberAmbiguity
        public new interface Base<TInheritTag> :
            IStateEnterAspect<TIdentity, TState>
          , IStateInheritable<TIdentity, TState, TInheritTag>
            where TInheritTag : unmanaged, IStateInheritTag<TIdentity, TState> { }
    }
}