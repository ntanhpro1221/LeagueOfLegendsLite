using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

public struct MoveData : IComponentData {
    [GhostField] public floatXZ_Q3 targetLocDir;
    [GhostField] public float_Q3   moveSpeed;
    [GhostField] public bool       controlYAxis;
    [GhostField] public bool       isMoveDone;
    [GhostField] public bool       isFixedPos;
    [GhostField] public float3_Q3  fixedPos;

    public void FixToPos(float3_Q3 pos) => (isFixedPos, fixedPos) = (true, pos);

    #region ROTATE

    private const float ROTATE_DIR_MIN_SQR = 1f;

    public static bool IsRotateDirValid(floatXZ_Q3 dir)
        => dir.LengthSqr() > ROTATE_DIR_MIN_SQR;

    public void RotateTo(floatXZ_Q3 dir) {
        if (IsRotateDirValid(dir)) targetLocDir = dir;
    }

    public void RotateTo(float3 yourPos, float3 targetPos)
        => RotateTo((targetPos - yourPos).Quantizate3().xz);

    public void RotateTo(float3 yourPos, Entity target, in ComponentLookup<LocalTransform> locTransLookup)
        => RotateTo(yourPos, locTransLookup[target].Position);

    #endregion
}

[GhostEnabledBit]
public struct MoveableTag : IComponentData, IEnableableComponent { }

public struct WaypointBuffer : IBufferElementData {
    [GhostField] public float3_Q3 pos;

    public WaypointBuffer(float3_Q3 _pos) => pos = _pos;
}

[GhostEnabledBit]
public struct NeedHandleWaypointRequest : IComponentData, IEnableableComponent { }

public struct WaypointRequestData : IComponentData {
    [GhostField] public float3_Q3 targetLocPos;

    /// <summary>
    /// Just for tmp calculating
    /// </summary>
    public int tmpPathId;
}

[RequireComponent(typeof(Rigidbody))]
public class MoveableAuthoring : MonoBehaviour {
    public new bool  enabled;
    public     bool  controlYAxis;
    public     float moveSpeed;

    private class Baker : ExtendBaker<MoveableAuthoring> {
        public override void Bake(MoveableAuthoring authoring) {
            GetDynamicEntity(out var entity);
            AddComponent(entity, new MoveData {
                controlYAxis = authoring.controlYAxis
              , moveSpeed    = authoring.moveSpeed.Quantizate3()
            });
            AddComponent<MoveableTag>(entity);
            SetComponentEnabled<MoveableTag>(entity, authoring.enabled);

            AddBuffer<WaypointBuffer>(entity);
            AddComponent<WaypointRequestData>(entity);
            AddComponentDisabled<NeedHandleWaypointRequest>(entity);
        }
    }
}