using NGDtuanh.Entities.StateMachine;

public class MonsterSharedStateAuthoring : ActorSharedStateAuthoring {
    protected class MonsterSharedStateBaker : InheritTagBaker<MonsterSharedStateAuthoring> {
        public override void MoreBake(MonsterSharedStateAuthoring authoring)
            => BakeActorSharedState<IdleState>(this);
    }

    protected override IStateInheritTag GetInheritTag(SharedAnimKey state, StateStep inheritAt)
        => (state, inheritAt) switch {
            // MOVE
            (SharedAnimKey.Move, StateStep.Exit)   => new MonsterStateMove.Exit.InheritTag()
          , (SharedAnimKey.Move, StateStep.Enter)  => new MonsterStateMove.Enter.InheritTag()
          , (SharedAnimKey.Move, StateStep.Update) => new MonsterStateMove.Update.InheritTag()
            // DEAD
          , (SharedAnimKey.Dead, StateStep.Exit)   => new MonsterStateDead.Exit.InheritTag()
          , (SharedAnimKey.Dead, StateStep.Enter)  => new MonsterStateDead.Enter.InheritTag()
          , (SharedAnimKey.Dead, StateStep.Update) => new MonsterStateDead.Update.InheritTag()
            // NULL
          , _ => null
        };
}