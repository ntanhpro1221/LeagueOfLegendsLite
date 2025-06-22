using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct MoveData : IComponentData {
    [GhostField] public float_Q3   moveSpeed;
    [GhostField] public bool       controlYAxis;
    [GhostField] public bool       isMoveDone;
    [GhostField] public bool       isFixedPos;
    [GhostField] public float3_Q3  fixedPos;

    public void FixToPos(float3_Q3 pos) => (isFixedPos, fixedPos) = (true, pos);
}

[GhostEnabledBit]
public struct MoveSpeedOverride : IComponentData, IEnableableComponent { }

public struct MoveSpeedOverrideData : IComponentData {
    [GhostField] public float_Q3 speed;
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
    [GhostField] public PathId pid;
}

[GhostEnabledBit]
public struct PathIsHandling : IComponentData, IEnableableComponent { }

public struct PathHandlingData : IComponentData {
    [GhostField] public NetworkTick doneAtTick;

    [GhostField] public PathId orgPID;
    [GhostField] public PathId newPID;

    public PathHandlingData(
        NetworkTick doneAtTick
      , float3_Q3   startPnt
      , float3_Q3   endPnt) {
        this.doneAtTick = doneAtTick;

        orgPID = newPID = new PathId(startPnt, endPnt);
    }
    
    public PathHandlingData(
        NetworkTick doneAtTick
      , PathId pid) {
        this.doneAtTick = doneAtTick;

        orgPID = newPID = pid;
    }

    public void UpdatePID(PathId pid) {
        newPID = pid;
    }
}

[RequireComponent(
    typeof(Rigidbody)
  , typeof(RotationAuthoring))]
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
            AddComponentDisabled<MoveSpeedOverride>(entity);
            AddComponent<MoveSpeedOverrideData>(entity);
            AddComponent<MoveableTag>(entity);
            SetComponentEnabled<MoveableTag>(entity, authoring.enabled);

            AddBuffer<WaypointBuffer>(entity);
            AddComponent<WaypointRequestData>(entity);
            AddComponentDisabled<NeedHandleWaypointRequest>(entity);

            AddComponent<PathHandlingData>(entity);
            AddComponentDisabled<PathIsHandling>(entity);
        }
    }
}