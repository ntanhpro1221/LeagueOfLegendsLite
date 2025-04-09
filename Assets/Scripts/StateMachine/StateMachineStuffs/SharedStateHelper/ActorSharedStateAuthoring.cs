using NGDtuanh.Entities.StateMachine;
using UnityEngine;

public class ActorSharedStateAuthoring : MonoBehaviour {
    public class Baker : ExtendBaker<ActorSharedStateAuthoring> {
        public override void Bake(ActorSharedStateAuthoring authoring) {
            GetDynamicEntity(out var entity);
            
            // Require for the state machine
            AddComponent<StateRequireEnter>(entity);
            AddComponent<StateNotExitedYet>(entity);
            
            // Idle: entry state
            AddComponent<IdleState>(entity);

            // Attack
            AddComponentDisabled<AttackState>(entity);
            AddComponent<AttackStateData>(entity);

            // Dead
            AddComponentDisabled<DeadState>(entity);
            AddComponent<DeadStateData>(entity);

            // Freeze
            AddComponentDisabled<FreezeState>(entity);

            // Move
            AddComponentDisabled<MoveState>(entity);
        }
    }
}