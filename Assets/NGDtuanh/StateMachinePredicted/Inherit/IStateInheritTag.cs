using Unity.Entities;

namespace NGDtuanh.Entities.StateMachine {
    public interface IStateInheritTag : IComponentData { }

    public interface IStateInheritTag<TIdentity, TState> : IStateInheritTag
        where TIdentity : unmanaged, IComponentData
        where TState : unmanaged, IComponentData, IEnableableComponent { }
}