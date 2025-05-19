using Unity.Entities;

namespace NGDtuanh.Entities.StateMachine {
    /// <typeparam name="TIdentity">Target identity</typeparam>
    /// <typeparam name="TState">Target state</typeparam>
    public interface IStateExitAspect<TIdentity, TState> : IStateAspect<TIdentity, TState>
        where TIdentity : unmanaged, IComponentData
        where TState : unmanaged, IComponentData, IEnableableComponent {
        RefRO<TState> IStateAspect<TIdentity, TState>.CurState => default; // we don't need this anymore

        protected EnabledRefRW<StateNotExitedYet> StateNotExitedYet { get; }
        protected EnabledRefRW<TState>            CurStateEnable    { get; }

        /// <summary>
        /// You must run this function to mark that state exit logic has been executed successfully.<br/>
        /// ==> So that <see cref="StateRequireEnter"/> will be enabled automatically.<br/>
        /// ==> So that other state will not run exit logic.<br/>
        /// <code>_stateNotExitedYet.ValueRW = _curStateEnable.ValueRW = false;</code>
        /// </summary>
        void MarkExitExecuted();

        // ReSharper disable once PossibleInterfaceMemberAmbiguity
        public new interface RequireInherit<TInheritTag> :
            IStateExitAspect<TIdentity, TState>
          , IStateInheritable<TIdentity, TState, TInheritTag>
            where TInheritTag : unmanaged, IStateInheritTag<TIdentity, TState> { }
    }
}