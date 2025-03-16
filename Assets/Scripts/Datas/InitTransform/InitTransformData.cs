using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct InitTransformData {
    public float3     position;
    public quaternion rotation;

    public InitTransformData(Transform transform) {
        position = transform.position;
        rotation = transform.rotation;
    }

    /// <summary>
    /// Because it uses <see cref="LocalToWorld"/> matrix (and it may be not up-to-date value) so it may be incorrect.<br/>
    /// If you need precise value use <see cref="ToLocalTransform_Precise"/>
    /// </summary>
    public LocalTransform ToLocalTransform_Fast(in LocalToWorld localToWorld) => LocalTransform.FromPositionRotation(
        localToWorld.Value.InverseTransformPoint(position)
      , localToWorld.Value.InverseTransformRotation(rotation)
    );

    /// <summary>
    /// Calculate very up-to-date because it uses <see cref="TransformHelpers.ComputeWorldTransformMatrix"/> but slower than <see cref="ToLocalTransform_Fast"/>.
    /// </summary>
    public LocalTransform ToLocalTransform_Precise(in  Entity                               entity
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

    public static implicit operator InitTransformData(Transform transform) => new(transform);
}