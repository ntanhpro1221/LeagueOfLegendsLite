using System.Collections.Generic;
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

    public static void AddBuffer<TBufferElement>(
        this IBaker                 baker
      , in   Entity                 entity
      , IEnumerable<TBufferElement> source)
        where TBufferElement : unmanaged, IBufferElementData {
        var buffer = baker.AddBuffer<TBufferElement>(entity);
        foreach (var item in source) buffer.Add(item);
    }

    public static void AddBufferDisabled<TBufferElement>(
        this IBaker                 baker
      , in   Entity                 entity
      , IEnumerable<TBufferElement> source)
        where TBufferElement : unmanaged, IBufferElementData, IEnableableComponent {
        baker.AddBuffer(entity, source);
        baker.SetComponentEnabled<TBufferElement>(entity, false);
    }

    public static void AddCleanBuffer<TBufferElement>(this IBaker baker, in Entity entity, int size)
        where TBufferElement : unmanaged, IBufferElementData {
        var buffer = baker.AddBuffer<TBufferElement>(entity);
        buffer.Resize(size, NativeArrayOptions.ClearMemory);
    }

    public static void AddCleanBufferDisabled<TBufferElement>(this IBaker baker, in Entity entity, int size)
        where TBufferElement : unmanaged, IBufferElementData, IEnableableComponent {
        baker.AddCleanBuffer<TBufferElement>(entity, size);
        baker.SetComponentEnabled<TBufferElement>(entity, false);
    }

    #endregion
}