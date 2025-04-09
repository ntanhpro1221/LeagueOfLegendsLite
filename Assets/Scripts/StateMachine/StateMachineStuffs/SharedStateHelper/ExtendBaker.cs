using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public abstract class ExtendBaker<TAuthoringType> : Baker<TAuthoringType>
    where TAuthoringType : Component {
    #region GET ENTITY

    public Entity GetDynamicEntity() => GetEntity(TransformUsageFlags.Dynamic);

    public Entity GetDynamicEntity(GameObject obj) => GetEntity(obj, TransformUsageFlags.Dynamic);

    public void GetDynamicEntity(out Entity entity) => entity = GetDynamicEntity();

    public void GetDynamicEntity(out Entity entity, GameObject obj) => entity = GetDynamicEntity(obj);

    #endregion
    
    #region ADD COMPONENT

    public void AddComponentDisabled<TComponent>(Entity entity, TComponent component)
        where TComponent : unmanaged, IComponentData, IEnableableComponent {
        AddComponent(entity, component);
        SetComponentEnabled<TComponent>(entity, false);
    }

    public void AddComponentDisabled<TComponent>(Entity entity)
        where TComponent : unmanaged, IEnableableComponent {
        AddComponent<TComponent>(entity);
        SetComponentEnabled<TComponent>(entity, false);
    }
    
    #endregion

    #region ADD BUFFER
    
    public void AddCleanBuffer<TBuffer>(in Entity entity, int size)
        where TBuffer : unmanaged, IBufferElementData {
        var buffer = AddBuffer<TBuffer>(entity);
        buffer.Resize(size, NativeArrayOptions.ClearMemory);
    }

    public void AddCleanBufferDisabled<TBuffer>(in Entity entity, int size)
        where TBuffer : unmanaged, IBufferElementData, IEnableableComponent {
        AddCleanBuffer<TBuffer>(entity, size);
        SetComponentEnabled<TBuffer>(entity, false);
    }
    
    #endregion
}