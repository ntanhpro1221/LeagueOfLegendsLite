public class ScuttleSharedStateAuthoring : MonsterSharedStateAuthoring {
    protected class ScuttleSharedStateBaker : InheritTagBaker<ScuttleSharedStateAuthoring> {
        public override void MoreBake(ScuttleSharedStateAuthoring authoring)
            => BakeActorSharedState<MoveState>(this);
    }
}