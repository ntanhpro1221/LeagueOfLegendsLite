using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public abstract class ExtendBaker<TAuthoringType> : Baker<TAuthoringType>
    where TAuthoringType : Component {
    #region GET ENTITY

    public Entity GetDynamicEntity() =>
        (this as IBaker).GetDynamicEntity();

    public Entity GetDynamicEntity(GameObject obj) =>
        (this as IBaker).GetDynamicEntity(obj);

    public void GetDynamicEntity(out Entity entity) =>
        (this as IBaker).GetDynamicEntity(out entity);

    public void GetDynamicEntity(out Entity entity, GameObject obj) =>
        (this as IBaker).GetDynamicEntity(out entity, obj);

    #endregion

    #region ADD COMPONENT

    public void AddComponentDisabled<TComponent>(Entity entity, TComponent component)
        where TComponent : unmanaged, IComponentData, IEnableableComponent =>
        (this as IBaker).AddComponentDisabled(entity, component);

    public void AddComponentDisabled<TComponent>(Entity entity)
        where TComponent : unmanaged, IEnableableComponent =>
        (this as IBaker).AddComponentDisabled<TComponent>(entity);

    #endregion

    #region ADD BUFFER

    public void AddCleanBuffer<TBuffer>(in Entity entity, int size)
        where TBuffer : unmanaged, IBufferElementData =>
        (this as IBaker).AddCleanBuffer<TBuffer>(entity, size);

    public void AddCleanBufferDisabled<TBuffer>(in Entity entity, int size)
        where TBuffer : unmanaged, IBufferElementData, IEnableableComponent =>
        (this as IBaker).AddCleanBufferDisabled<TBuffer>(entity, size);

    #endregion
}