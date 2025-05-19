using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Entities;

public static partial class TowerStateDead {
    [UpdateInGroup(typeof(StateEnterSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct Enter : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (_, anim, select_highlight_healthBar) in SystemAPI.Query<
                StateFilterAspect
              , SharedAnimAspect
              , Select_Highlight_HealthBarAspect>()) {
                anim.SetAnim(SharedAnimKey.Dead);

                select_highlight_healthBar.DisableAll();
            }
        }
    }
}

public static partial class TowerStateDead {
    public partial struct Enter {
        public struct InheritTag : IStateInheritTag<TowerTag, DeadState> { }

        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<TowerTag, DeadState>.RequireInherit<InheritTag> {
            private readonly RefRO<TowerTag>  _identity;
            private readonly RefRO<DeadState> _curState;
            private readonly RefRO<Simulate>  _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<TowerTag> IStateAspect<TowerTag, DeadState>. Identity => _identity;
            RefRO<DeadState> IStateAspect<TowerTag, DeadState>.CurState => _curState;
            RefRO<Simulate> IStateAspect<TowerTag, DeadState>. Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<TowerTag, DeadState>.StateRequireEnter => _stateRequireEnter;

            private readonly RefRO<InheritTag>                                   _inheritTag;
            RefRO<InheritTag> IStateInheritable<TowerTag, DeadState, InheritTag>.InheritTag => _inheritTag;
        }
    }
}