using NGDtuanh.Entities.StateMachine;

public class TowerSharedStateAuthoring : ActorSharedStateAuthoring {
    protected class TowerSharedStateBaker : InheritTagBaker<TowerSharedStateAuthoring> {
        public override void MoreBake(TowerSharedStateAuthoring authoring)
            => BakeActorSharedState(this);
    }

    protected override IStateInheritTag GetInheritTag(SharedAnimKey state, StateStep inheritAt)
        => (state, inheritAt) switch {
            (SharedAnimKey.Idle, StateStep.Exit)  => new TowerStateIdle.Exit.InheritTag()
          , (SharedAnimKey.Dead, StateStep.Enter) => new TowerStateDead.Enter.InheritTag()
          , _                                     => null
        };
}