using NGDtuanh.Entities.StateMachine;
using Unity.Entities;
using UnityEngine;

public static class BakeHelper {
    public static void AddActorSharedState<TAuthoring>(
        this ExtendBaker<TAuthoring> baker
      , in   Entity                  entity)
        where TAuthoring : Component {
        baker.AddComponent<StateRequireEnter>(entity);
        baker.AddComponent<StateNotExitedYet>(entity);
        baker.AddComponent<IdleState>(entity);

        baker.AddComponentDisabled<AttackState>(entity);
        baker.AddComponent<AttackStateData>(entity);

        baker.AddComponentDisabled<DeadState>(entity);
        baker.AddComponent<DeadStateData>(entity);

        baker.AddComponentDisabled<FreezeState>(entity);


        baker.AddComponentDisabled<MoveState>(entity);
    }

    /// <summary>
    /// Inherit from <see cref="AddActorSharedState{TAuthoring}"/>
    /// </summary>
    public static void AddLivingActorSharedState<TAuthoring>(
        this ExtendBaker<TAuthoring> baker
      , in   Entity                  entity)
        where TAuthoring : Component {
        baker.AddActorSharedState(entity);
    }
}