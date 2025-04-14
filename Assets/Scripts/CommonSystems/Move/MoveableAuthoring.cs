using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

public struct MoveData : IComponentData {
    private const float ROTATE_DIR_MIN_SQR = 1f;

    [GhostField] public float3_Q3  targetLocPos;
    [GhostField] public floatXZ_Q3 targetLocDir;
    [GhostField] public float_Q3   moveSpeed;
    [GhostField] public bool       isMoveDone;
    [GhostField] public bool       controlYAxis;

    public void MoveTo(float3_Q3 pos) => (targetLocPos, isMoveDone) = (pos, false);
    public void TeleTo(float3_Q3 pos) => (targetLocPos, isMoveDone) = (pos, true);

    public void RotateTo(floatXZ_Q3 dir) {
        if (IsRotateDirValid(dir)) targetLocDir = dir;
    }

    public void RotateTo(float3 yourPos, float3 targetPos)
        => RotateTo((targetPos - yourPos).Quantizate3().xz);

    public void RotateTo(float3 yourPos, Entity target, in ComponentLookup<LocalTransform> locTransLookup)
        => RotateTo(yourPos, locTransLookup[target].Position);

    public void SyncFromLocTrans(in LocalTransform locTrans) {
        TeleTo(locTrans.Position.Quantizate3());
        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        targetLocDir = locTrans.Forward().Quantizate3().xz;
    }

    public void MarkMoveDone() => isMoveDone = true;

    public static bool IsRotateDirValid(floatXZ_Q3 dir) => dir.LengthSqr() > ROTATE_DIR_MIN_SQR;
}

[GhostEnabledBit]
public struct MoveableTag : IComponentData, IEnableableComponent { }

[RequireComponent(typeof(Rigidbody))]
public class MoveableAuthoring : MonoBehaviour {
    public new bool  enabled;
    public     bool  controlYAxis;
    public     float moveSpeed;

    private class Baker : ExtendBaker<MoveableAuthoring> {
        public override void Bake(MoveableAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveData {
                controlYAxis = authoring.controlYAxis
              , moveSpeed    = authoring.moveSpeed.Quantizate3()
            });
            AddComponent<MoveableTag>(entity);
            SetComponentEnabled<MoveableTag>(entity, authoring.enabled);
        }
    }
}