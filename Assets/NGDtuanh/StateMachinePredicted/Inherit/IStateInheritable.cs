using Unity.Entities;

namespace NGDtuanh.Entities.StateMachine {
    public interface IStateInheritable<TIdentity, TState, TInheritTag>
        where TIdentity : unmanaged, IComponentData
        where TState : unmanaged, IComponentData, IEnableableComponent
        where TInheritTag : unmanaged, IStateInheritTag<TIdentity, TState> {
        protected RefRO<TInheritTag> InheritTag { get; }
    }
}