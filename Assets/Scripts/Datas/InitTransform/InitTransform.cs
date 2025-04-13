using NGDtuanh.BubleAsset;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public struct InitTransform : IBlobBuildable<Transform>, IBlobBuildableSelf<InitTransform> {
    public float3_Q3 position;
    public float4_Q3 rotation;

    public InitTransform(Transform transform) {
        position = (float3_Q3)transform.position;
        rotation = (float4_Q3)transform.rotation;
    }

    public LocalTransform ToLocTrans_Directly()
        => LocalTransform.FromPositionRotation(
            position
          , rotation);

    /// <summary>
    /// Because it uses <see cref="LocalToWorld"/> matrix (and it may be not up-to-date value) so it may be incorrect.<br/>
    /// If you need precise value use <see cref="ToLocTrans_Precise"/>
    /// </summary>
    public LocalTransform ToLocTrans_Fast(in LocalToWorld localToWorld)
        => LocalTransform.FromPositionRotation(
            localToWorld.Value.InverseTransformPoint(position)
          , localToWorld.Value.InverseTransformRotation(rotation)
        );

    /// <summary>
    /// Calculate very up to date because it uses <see cref="TransformHelpers.ComputeWorldTransformMatrix"/> but slower than <see cref="ToLocTrans_Fast"/>.
    /// </summary>
    public LocalTransform ToLocTrans_Precise(
        in  Entity                               entity
      , ref ComponentLookup<LocalTransform>      localTransformLookup
      , ref ComponentLookup<Parent>              parentLookup
      , ref ComponentLookup<PostTransformMatrix> scaleLookup) {
        TransformHelpers.ComputeWorldTransformMatrix(
            entity
          , out var localToWorld
          , ref localTransformLookup
          , ref parentLookup
          , ref scaleLookup);
        return LocalTransform.FromPositionRotation(
            localToWorld.InverseTransformPoint(position)
          , localToWorld.InverseTransformRotation(rotation)
        );
    }

    public static implicit operator InitTransform(Transform transform) => new(transform);

    public void BuildBlob(ref BlobBuilder builder, Transform source)
        => this = source;

    public void BuildBlob(ref BlobBuilder builder, ref InitTransform source)
        => this = source;
}