using NGDtuanh.Entities.StateMachine;
using Unity.Collections;
using Unity.Entities;

public static class BakeHelper {
    public static void AddComponentDisabled<TComponent>(
        this IBaker baker
      , in   Entity entity)
        where TComponent : unmanaged, IEnableableComponent {
        baker.AddComponent<TComponent>(entity);
        baker.SetComponentEnabled<TComponent>(entity, false);
    }

    public static void AddComponentDisabled<TComponent>(
        this IBaker baker
      , in   Entity entity
      , TComponent  component)
        where TComponent : unmanaged, IComponentData, IEnableableComponent {
        baker.AddComponent(entity, component);
        baker.SetComponentEnabled<TComponent>(entity, false);
    }

    public static void AddBufferDisabled<TBuffer>(
        this IBaker baker
      , in   Entity entity
      , int         size)
        where TBuffer : unmanaged, IBufferElementData, IEnableableComponent {
        baker.AddBuffer<TBuffer>(entity, size);
        baker.SetComponentEnabled<TBuffer>(entity, false);
    }

    public static void AddBuffer<TBuffer>(
        this IBaker baker
      , in   Entity entity
      , int         size)
        where TBuffer : unmanaged, IBufferElementData {
        var buffer = baker.AddBuffer<TBuffer>(entity);
        buffer.Resize(size, NativeArrayOptions.ClearMemory);
    }

    public static void AddActorSharedState(
        this IBaker baker
      , in   Entity entity) {
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
    /// Inherit from <see cref="AddActorSharedState"/>
    /// </summary>
    public static void AddChampionSharedState(
        this IBaker baker
      , in   Entity entity) {
        baker.AddActorSharedState(entity);
    }
}