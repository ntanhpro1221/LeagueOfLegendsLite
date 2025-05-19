using Unity.Entities;

namespace NGDtuanh.Entities.StateMachine {
    public interface IStateAspect<TIdentity, TState>
        where TIdentity : unmanaged, IComponentData
        where TState : unmanaged, IComponentData, IEnableableComponent {
        protected RefRO<TIdentity> Identity { get; }
        protected RefRO<TState>    CurState { get; }
        protected RefRO<Simulate>  Simulate { get; }

        public interface RequireInherit<TInheritTag> :
            IStateAspect<TIdentity, TState>
          , IStateInheritable<TIdentity, TState, TInheritTag>
            where TInheritTag : unmanaged, IStateInheritTag<TIdentity, TState> { }
    }
}