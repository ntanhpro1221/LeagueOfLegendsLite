using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct MoveRequesterAspect : IAspect {
    private readonly RefRW<MoveData>               _MoveData;
    private readonly DynamicBuffer<WaypointBuffer> _Waypoints;
    private readonly RefRW<WaypointRequestData>    _RequestData;
    private readonly RefRW<PathHandlingData>       _HandlingData;
    private readonly RefRW<RotationData>           _RotationData;

    [Optional] private readonly EnabledRefRW<NeedHandleWaypointRequest> _RequestTrigger;
    [Optional] private readonly EnabledRefRW<PathIsHandling>            _HandlingTrigger;

    public bool IsMoveDone =>
        _MoveData.ValueRO.isMoveDone
     && !_HandlingTrigger.ValueRO;

    public bool HandlingTrigger => _HandlingTrigger.ValueRO;

    public bool WaypointIsEmpty =>
        !_HandlingTrigger.ValueRO
     && _Waypoints.IsEmpty;

    public float3_Q3 WaypointDestination => HandlingTrigger
        ? _HandlingData.ValueRO.newPID.end
        : _Waypoints[0].pos;

    public bool NeedRecalculatePath(float3 newDes) =>
        // Not have waypoint or
        WaypointIsEmpty
        // Des of waypoint if too far to expected des
     || GameHelpers.DistanceXZ_Sqr(WaypointDestination, newDes) > 1;

    /// <summary>
    /// This method is high-performance
    /// </summary>
    public void MoveStraightTo(float3_Q3 targetLocPos) {
        _MoveData.ValueRW.isMoveDone = false;
        _HandlingTrigger.ValueRW     = false;
        _RequestTrigger.ValueRW      = false;

        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        _Waypoints.Resize(1, NativeArrayOptions.ClearMemory);
        _Waypoints.FrontRW().pos = targetLocPos;
    }

    /// <summary>
    /// This method is expensive because it has to run pathfinding algorithm.
    /// (Try to use it as sparingly as possible)
    /// </summary>
    public void MoveSmartTo(float3_Q3 targetLocPos, in LocalTransform yourLocTrans) {
        _MoveData.ValueRW.isMoveDone = false;
        _RequestTrigger.ValueRW      = true;

        _RequestData.ValueRW.pid = new PathId(
            yourLocTrans.Position.Quantizate3()
          , targetLocPos);
    }

    public void TeleTo(float3_Q3 targetLocPos) {
        _MoveData.ValueRW.isMoveDone = true;
        _HandlingTrigger.ValueRW     = false;

        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        _Waypoints.Clear();

        _RequestTrigger.ValueRW    = false;
        _MoveData.ValueRW.fixedPos = targetLocPos;
    }

    public void SyncFromLocTrans(in LocalTransform locTrans) {
        TeleTo(locTrans.Position.Quantizate3());

        _RotationData.ValueRW.StopRotate();
    }
}