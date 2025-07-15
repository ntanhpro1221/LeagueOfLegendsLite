using NGDtuanh.Entities.StateMachine;

public class ChampionSharedStateAuthoring : ActorSharedStateAuthoring {
    protected class ChampionSharedStateBaker : InheritTagBaker<ChampionSharedStateAuthoring> {
        public override void MoreBake(ChampionSharedStateAuthoring authoring)
            => BakeActorSharedState<IdleState>(this);
    }

    protected override IStateInheritTag GetInheritTag(SharedAnimKey state, StateStep inheritAt)
        => (state, inheritAt) switch {
            (SharedAnimKey.Attack, StateStep.Enter) => new ChampionStateAttack.Enter.InheritTag()
          , _                                       => null
        };
}