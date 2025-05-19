using NGDtuanh.Entities.StateMachine;
using Unity.Entities;
using UnityEngine;

public class ActorSharedStateAuthoring : IAllStateAuthoring<SharedAnimKey> {
    protected static void BakeActorSharedState<TEntryState>(IBaker baker)
        where TEntryState : struct, IEnableableComponent {
        baker.GetDynamicEntity(out var entity);

        // Require for the state machine
        baker.AddComponent<StateRequireEnter>(entity);
        baker.AddComponent<StateNotExitedYet>(entity);
        baker.AddComponent<TransitionStateData>(entity);

        // Idle
        baker.AddComponentDisabled<IdleState>(entity);

        // Attack
        baker.AddComponentDisabled<AttackState>(entity);
        baker.AddComponent<AttackStateData>(entity);

        // Dead
        baker.AddComponentDisabled<DeadState>(entity);
        baker.AddComponent<DeadStateData>(entity);

        // Freeze
        baker.AddComponentDisabled<FreezeState>(entity);

        // Move
        baker.AddComponentDisabled<MoveState>(entity);
        
        // Dead2Idle
        baker.AddComponentDisabled<Dead2IdleState>(entity);
        
        // Idle2Dead
        baker.AddComponentDisabled<Idle2DeadState>(entity);
        
        baker.SetComponentEnabled<TEntryState>(entity, true);
    }

    protected class ActorSharedStateBaker : InheritTagBaker<ActorSharedStateAuthoring> {
        public override void MoreBake(ActorSharedStateAuthoring authoring)
            => BakeActorSharedState<IdleState>(this);
    }

    protected override IStateInheritTag GetInheritTag(SharedAnimKey state, StateStep inheritAt)
        => null;
}