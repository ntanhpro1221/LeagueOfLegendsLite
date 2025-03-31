using Unity.Entities;

namespace NGDtuanh.Entities.StateMachine {
    public interface IStateAspect<TIdentity, TState>
        where TIdentity : unmanaged, IComponentData
        where TState : unmanaged, IComponentData, IEnableableComponent {
        protected RefRO<TIdentity> Identity { get; }
        protected RefRO<TState>    CurState { get; }
        protected RefRO<Simulate>  Simulate { get; }
    }
}