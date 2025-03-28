using Unity.Entities;
using UnityEngine;

public abstract class DisabledTagBaker<TAuthoringType, TTag1> :
    Baker<TAuthoringType>
    where TAuthoringType : Component
    where TTag1 : struct, IEnableableComponent
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
    where TTag1 : struct, IEnableableComponent
    where TTag2 : struct, IEnableableComponent
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
    where TTag1 : struct, IEnableableComponent
    where TTag2 : struct, IEnableableComponent
    where TTag3 : struct, IEnableableComponent
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
    where TTag1 : struct, IEnableableComponent
    where TTag2 : struct, IEnableableComponent
    where TTag3 : struct, IEnableableComponent
    where TTag4 : struct, IEnableableComponent
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
    where TTag1 : struct, IEnableableComponent
    where TTag2 : struct, IEnableableComponent
    where TTag3 : struct, IEnableableComponent
    where TTag4 : struct, IEnableableComponent
    where TTag5 : struct, IEnableableComponent
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
    where TTag1 : struct, IEnableableComponent
    where TTag2 : struct, IEnableableComponent
    where TTag3 : struct, IEnableableComponent
    where TTag4 : struct, IEnableableComponent
    where TTag5 : struct, IEnableableComponent
    where TTag6 : struct, IEnableableComponent
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
    where TTag1 : struct, IEnableableComponent
    where TTag2 : struct, IEnableableComponent
    where TTag3 : struct, IEnableableComponent
    where TTag4 : struct, IEnableableComponent
    where TTag5 : struct, IEnableableComponent
    where TTag6 : struct, IEnableableComponent
    where TTag7 : struct, IEnableableComponent
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
    where TTag1 : struct, IEnableableComponent
    where TTag2 : struct, IEnableableComponent
    where TTag3 : struct, IEnableableComponent
    where TTag4 : struct, IEnableableComponent
    where TTag5 : struct, IEnableableComponent
    where TTag6 : struct, IEnableableComponent
    where TTag7 : struct, IEnableableComponent
    where TTag8 : struct, IEnableableComponent
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

