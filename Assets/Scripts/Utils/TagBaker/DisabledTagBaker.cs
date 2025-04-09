using Unity.Entities;
using UnityEngine;

public abstract class DisabledTagBaker<TAuthoringType, TTag1> :
    ExtendBaker<TAuthoringType>
    where TAuthoringType : Component
    where TTag1 : unmanaged, IEnableableComponent
{
    public override void Bake(TAuthoringType authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponentDisabled<TTag1>(entity);
    }
}
public abstract class DisabledTagBaker<TAuthoringType, TTag1, TTag2> :
    ExtendBaker<TAuthoringType>
    where TAuthoringType : Component
    where TTag1 : unmanaged, IEnableableComponent
    where TTag2 : unmanaged, IEnableableComponent
{
    public override void Bake(TAuthoringType authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponentDisabled<TTag1>(entity);
        AddComponentDisabled<TTag2>(entity);
    }
}
public abstract class DisabledTagBaker<TAuthoringType, TTag1, TTag2, TTag3> :
    ExtendBaker<TAuthoringType>
    where TAuthoringType : Component
    where TTag1 : unmanaged, IEnableableComponent
    where TTag2 : unmanaged, IEnableableComponent
    where TTag3 : unmanaged, IEnableableComponent
{
    public override void Bake(TAuthoringType authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponentDisabled<TTag1>(entity);
        AddComponentDisabled<TTag2>(entity);
        AddComponentDisabled<TTag3>(entity);
    }
}
public abstract class DisabledTagBaker<TAuthoringType, TTag1, TTag2, TTag3, TTag4> :
    ExtendBaker<TAuthoringType>
    where TAuthoringType : Component
    where TTag1 : unmanaged, IEnableableComponent
    where TTag2 : unmanaged, IEnableableComponent
    where TTag3 : unmanaged, IEnableableComponent
    where TTag4 : unmanaged, IEnableableComponent
{
    public override void Bake(TAuthoringType authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponentDisabled<TTag1>(entity);
        AddComponentDisabled<TTag2>(entity);
        AddComponentDisabled<TTag3>(entity);
        AddComponentDisabled<TTag4>(entity);
    }
}
public abstract class DisabledTagBaker<TAuthoringType, TTag1, TTag2, TTag3, TTag4, TTag5> :
    ExtendBaker<TAuthoringType>
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
        AddComponentDisabled<TTag1>(entity);
        AddComponentDisabled<TTag2>(entity);
        AddComponentDisabled<TTag3>(entity);
        AddComponentDisabled<TTag4>(entity);
        AddComponentDisabled<TTag5>(entity);
    }
}
public abstract class DisabledTagBaker<TAuthoringType, TTag1, TTag2, TTag3, TTag4, TTag5, TTag6> :
    ExtendBaker<TAuthoringType>
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
        AddComponentDisabled<TTag1>(entity);
        AddComponentDisabled<TTag2>(entity);
        AddComponentDisabled<TTag3>(entity);
        AddComponentDisabled<TTag4>(entity);
        AddComponentDisabled<TTag5>(entity);
        AddComponentDisabled<TTag6>(entity);
    }
}
public abstract class DisabledTagBaker<TAuthoringType, TTag1, TTag2, TTag3, TTag4, TTag5, TTag6, TTag7> :
    ExtendBaker<TAuthoringType>
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
        AddComponentDisabled<TTag1>(entity);
        AddComponentDisabled<TTag2>(entity);
        AddComponentDisabled<TTag3>(entity);
        AddComponentDisabled<TTag4>(entity);
        AddComponentDisabled<TTag5>(entity);
        AddComponentDisabled<TTag6>(entity);
        AddComponentDisabled<TTag7>(entity);
    }
}
public abstract class DisabledTagBaker<TAuthoringType, TTag1, TTag2, TTag3, TTag4, TTag5, TTag6, TTag7, TTag8> :
    ExtendBaker<TAuthoringType>
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
        AddComponentDisabled<TTag1>(entity);
        AddComponentDisabled<TTag2>(entity);
        AddComponentDisabled<TTag3>(entity);
        AddComponentDisabled<TTag4>(entity);
        AddComponentDisabled<TTag5>(entity);
        AddComponentDisabled<TTag6>(entity);
        AddComponentDisabled<TTag7>(entity);
        AddComponentDisabled<TTag8>(entity);
    }
}

