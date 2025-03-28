using Unity.Entities;
using UnityEngine;

public abstract class TagBaker<TAuthoringType, TTag1> :
    Baker<TAuthoringType>
    where TAuthoringType : Component {
    public override void Bake(TAuthoringType authoring) {
        AddComponent<TTag1>(GetEntity(TransformUsageFlags.Dynamic));
    }
}
public abstract class TagBaker<TAuthoringType, TTag1, TTag2> :
    Baker<TAuthoringType>
    where TAuthoringType : Component {
    public override void Bake(TAuthoringType authoring) {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<TTag1>(entity);
        AddComponent<TTag2>(entity);
    }
}
public abstract class TagBaker<TAuthoringType, TTag1, TTag2, TTag3> :
    Baker<TAuthoringType>
    where TAuthoringType : Component {
    public override void Bake(TAuthoringType authoring) {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<TTag1>(entity);
        AddComponent<TTag2>(entity);
        AddComponent<TTag3>(entity);
    }
}
public abstract class TagBaker<TAuthoringType, TTag1, TTag2, TTag3, TTag4> :
    Baker<TAuthoringType>
    where TAuthoringType : Component {
    public override void Bake(TAuthoringType authoring) {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<TTag1>(entity);
        AddComponent<TTag2>(entity);
        AddComponent<TTag3>(entity);
        AddComponent<TTag4>(entity);
    }
}
public abstract class TagBaker<TAuthoringType, TTag1, TTag2, TTag3, TTag4, TTag5> :
    Baker<TAuthoringType>
    where TAuthoringType : Component {
    public override void Bake(TAuthoringType authoring) {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<TTag1>(entity);
        AddComponent<TTag2>(entity);
        AddComponent<TTag3>(entity);
        AddComponent<TTag4>(entity);
        AddComponent<TTag5>(entity);
    }
}
public abstract class TagBaker<TAuthoringType, TTag1, TTag2, TTag3, TTag4, TTag5, TTag6> :
    Baker<TAuthoringType>
    where TAuthoringType : Component {
    public override void Bake(TAuthoringType authoring) {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<TTag1>(entity);
        AddComponent<TTag2>(entity);
        AddComponent<TTag3>(entity);
        AddComponent<TTag4>(entity);
        AddComponent<TTag5>(entity);
        AddComponent<TTag6>(entity);
    }
}
public abstract class TagBaker<TAuthoringType, TTag1, TTag2, TTag3, TTag4, TTag5, TTag6, TTag7> :
    Baker<TAuthoringType>
    where TAuthoringType : Component {
    public override void Bake(TAuthoringType authoring) {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<TTag1>(entity);
        AddComponent<TTag2>(entity);
        AddComponent<TTag3>(entity);
        AddComponent<TTag4>(entity);
        AddComponent<TTag5>(entity);
        AddComponent<TTag6>(entity);
        AddComponent<TTag7>(entity);
    }
}
public abstract class TagBaker<TAuthoringType, TTag1, TTag2, TTag3, TTag4, TTag5, TTag6, TTag7, TTag8> :
    Baker<TAuthoringType>
    where TAuthoringType : Component {
    public override void Bake(TAuthoringType authoring) {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<TTag1>(entity);
        AddComponent<TTag2>(entity);
        AddComponent<TTag3>(entity);
        AddComponent<TTag4>(entity);
        AddComponent<TTag5>(entity);
        AddComponent<TTag6>(entity);
        AddComponent<TTag7>(entity);
        AddComponent<TTag8>(entity);
    }
}