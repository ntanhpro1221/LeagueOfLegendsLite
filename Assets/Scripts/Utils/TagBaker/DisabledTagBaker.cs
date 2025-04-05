using Unity.Entities;
using UnityEngine;

public abstract class DisabledTagBaker<TAuthoringType, TTag1> :
    Baker<TAuthoringType>
    where TAuthoringType : Component
    where TTag1 : unmanaged, IEnableableComponent
{
    public override void Bake(TAuthoringType authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        this.AddComponentDisabled<TTag1>(entity);
    }
}
public abstract class DisabledTagBaker<TAuthoringType, TTag1, TTag2> :
    Baker<TAuthoringType>
    where TAuthoringType : Component
    where TTag1 : unmanaged, IEnableableComponent
    where TTag2 : unmanaged, IEnableableComponent
{
    public override void Bake(TAuthoringType authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        this.AddComponentDisabled<TTag1>(entity);
        this.AddComponentDisabled<TTag2>(entity);
    }
}
public abstract class DisabledTagBaker<TAuthoringType, TTag1, TTag2, TTag3> :
    Baker<TAuthoringType>
    where TAuthoringType : Component
    where TTag1 : unmanaged, IEnableableComponent
    where TTag2 : unmanaged, IEnableableComponent
    where TTag3 : unmanaged, IEnableableComponent
{
    public override void Bake(TAuthoringType authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        this.AddComponentDisabled<TTag1>(entity);
        this.AddComponentDisabled<TTag2>(entity);
        this.AddComponentDisabled<TTag3>(entity);
    }
}
public abstract class DisabledTagBaker<TAuthoringType, TTag1, TTag2, TTag3, TTag4> :
    Baker<TAuthoringType>
    where TAuthoringType : Component
    where TTag1 : unmanaged, IEnableableComponent
    where TTag2 : unmanaged, IEnableableComponent
    where TTag3 : unmanaged, IEnableableComponent
    where TTag4 : unmanaged, IEnableableComponent
{
    public override void Bake(TAuthoringType authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        this.AddComponentDisabled<TTag1>(entity);
        this.AddComponentDisabled<TTag2>(entity);
        this.AddComponentDisabled<TTag3>(entity);
        this.AddComponentDisabled<TTag4>(entity);
    }
}
public abstract class DisabledTagBaker<TAuthoringType, TTag1, TTag2, TTag3, TTag4, TTag5> :
    Baker<TAuthoringType>
    where TAuthoringType : Component
    where TTag1 : unmanaged, IEnableableComponent
    where TTag2 : unmanaged, IEnableableComponent
    where TTag3 : unmanaged, IEnableableComponent
    where TTag4 : unmanaged, IEnableableComponent
    where TTag5 : unmanaged, IEnableableComponent
{
    public override void Bake(TAuthoringType authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        this.AddComponentDisabled<TTag1>(entity);
        this.AddComponentDisabled<TTag2>(entity);
        this.AddComponentDisabled<TTag3>(entity);
        this.AddComponentDisabled<TTag4>(entity);
        this.AddComponentDisabled<TTag5>(entity);
    }
}
public abstract class DisabledTagBaker<TAuthoringType, TTag1, TTag2, TTag3, TTag4, TTag5, TTag6> :
    Baker<TAuthoringType>
    where TAuthoringType : Component
    where TTag1 : unmanaged, IEnableableComponent
    where TTag2 : unmanaged, IEnableableComponent
    where TTag3 : unmanaged, IEnableableComponent
    where TTag4 : unmanaged, IEnableableComponent
    where TTag5 : unmanaged, IEnableableComponent
    where TTag6 : unmanaged, IEnableableComponent
{
    public override void Bake(TAuthoringType authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        this.AddComponentDisabled<TTag1>(entity);
        this.AddComponentDisabled<TTag2>(entity);
        this.AddComponentDisabled<TTag3>(entity);
        this.AddComponentDisabled<TTag4>(entity);
        this.AddComponentDisabled<TTag5>(entity);
        this.AddComponentDisabled<TTag6>(entity);
    }
}
public abstract class DisabledTagBaker<TAuthoringType, TTag1, TTag2, TTag3, TTag4, TTag5, TTag6, TTag7> :
    Baker<TAuthoringType>
    where TAuthoringType : Component
    where TTag1 : unmanaged, IEnableableComponent
    where TTag2 : unmanaged, IEnableableComponent
    where TTag3 : unmanaged, IEnableableComponent
    where TTag4 : unmanaged, IEnableableComponent
    where TTag5 : unmanaged, IEnableableComponent
    where TTag6 : unmanaged, IEnableableComponent
    where TTag7 : unmanaged, IEnableableComponent
{
    public override void Bake(TAuthoringType authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        this.AddComponentDisabled<TTag1>(entity);
        this.AddComponentDisabled<TTag2>(entity);
        this.AddComponentDisabled<TTag3>(entity);
        this.AddComponentDisabled<TTag4>(entity);
        this.AddComponentDisabled<TTag5>(entity);
        this.AddComponentDisabled<TTag6>(entity);
        this.AddComponentDisabled<TTag7>(entity);
    }
}
public abstract class DisabledTagBaker<TAuthoringType, TTag1, TTag2, TTag3, TTag4, TTag5, TTag6, TTag7, TTag8> :
    Baker<TAuthoringType>
    where TAuthoringType : Component
    where TTag1 : unmanaged, IEnableableComponent
    where TTag2 : unmanaged, IEnableableComponent
    where TTag3 : unmanaged, IEnableableComponent
    where TTag4 : unmanaged, IEnableableComponent
    where TTag5 : unmanaged, IEnableableComponent
    where TTag6 : unmanaged, IEnableableComponent
    where TTag7 : unmanaged, IEnableableComponent
    where TTag8 : unmanaged, IEnableableComponent
{
    public override void Bake(TAuthoringType authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        this.AddComponentDisabled<TTag1>(entity);
        this.AddComponentDisabled<TTag2>(entity);
        this.AddComponentDisabled<TTag3>(entity);
        this.AddComponentDisabled<TTag4>(entity);
        this.AddComponentDisabled<TTag5>(entity);
        this.AddComponentDisabled<TTag6>(entity);
        this.AddComponentDisabled<TTag7>(entity);
        this.AddComponentDisabled<TTag8>(entity);
    }
}

