using NGDtuanh.Entities.StateMachine;
using Unity.Collections;
using Unity.Entities;

public static class BakeHelper {
    public static void AddComponentDisabled<TComponent>(
        this IBaker baker
      , in   Entity entity)
        where TComponent : struct, IEnableableComponent {
        baker.AddComponent<TComponent>(entity);
        baker.SetComponentEnabled<TComponent>(entity, false);
    }

    public static void AddBufferDisabled<TBuffer>(
        this IBaker baker
      , in   Entity entity
      , int         size)
        where TBuffer : unmanaged, IBufferElementData, IEnableableComponent {
        var buffer = baker.AddBuffer<TBuffer>(entity);
        buffer.Resize(size, NativeArrayOptions.ClearMemory);
        baker.SetComponentEnabled<TBuffer>(entity, false);
    }

    public static void AddDefaultSharedState(
        this IBaker baker
      , in   Entity entity) {
        baker.AddComponentDisabled<StateNeedEnterTag>(entity);
        baker.AddComponent<EntryState>(entity);
    }

    /// <summary>
    /// Inherit from <see cref="AddDefaultSharedState"/>
    /// </summary>
    public static void AddActorSharedState(
        this IBaker baker
      , in   Entity entity) {
        baker.AddDefaultSharedState(entity);

        baker.AddComponentDisabled<AttackState>(entity);
        baker.AddComponentDisabled<DeadState>(entity);
        baker.AddComponentDisabled<FreezeState>(entity);
        baker.AddComponentDisabled<IdleState>(entity);
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