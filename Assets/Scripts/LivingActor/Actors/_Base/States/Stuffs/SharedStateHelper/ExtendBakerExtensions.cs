using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public static class ExtendBakerExtensions {
    #region GET ENTITY

    public static Entity GetDynamicEntity(this IBaker baker) =>
        baker.GetEntity(TransformUsageFlags.Dynamic);

    public static Entity GetDynamicEntity(this IBaker baker, GameObject obj) =>
        baker.GetEntity(obj, TransformUsageFlags.Dynamic);

    public static void GetDynamicEntity(this IBaker baker, out Entity entity) =>
        entity = baker.GetDynamicEntity();

    public static void GetDynamicEntity(this IBaker baker, out Entity entity, GameObject obj) =>
        entity = baker.GetDynamicEntity(obj);

    #endregion

    #region ADD COMPONENT

    public static void AddComponentDisabled<TComponent>(this IBaker baker, Entity entity, TComponent component)
        where TComponent : unmanaged, IComponentData, IEnableableComponent {
        baker.AddComponent(entity, component);
        baker.SetComponentEnabled<TComponent>(entity, false);
    }

    public static void AddComponentDisabled<TComponent>(this IBaker baker, Entity entity)
        where TComponent : unmanaged, IEnableableComponent {
        baker.AddComponent<TComponent>(entity);
        baker.SetComponentEnabled<TComponent>(entity, false);
    }

    #endregion

    #region ADD BUFFER

    public static void AddCleanBuffer<TBuffer>(this IBaker baker, in Entity entity, int size)
        where TBuffer : unmanaged, IBufferElementData {
        var buffer = baker.AddBuffer<TBuffer>(entity);
        buffer.Resize(size, NativeArrayOptions.ClearMemory);
    }

    public static void AddCleanBufferDisabled<TBuffer>(this IBaker baker, in Entity entity, int size)
        where TBuffer : unmanaged, IBufferElementData, IEnableableComponent {
        baker.AddCleanBuffer<TBuffer>(entity, size);
        baker.SetComponentEnabled<TBuffer>(entity, false);
    }

    #endregion
}