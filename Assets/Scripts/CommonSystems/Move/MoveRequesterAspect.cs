using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct MoveRequesterAspect : IAspect {
    private readonly RefRW<MoveData>               _MoveData;
    private readonly DynamicBuffer<WaypointBuffer> _WaypointBuffer;
    private readonly RefRW<WaypointRequestData>    _WaypointRequest;

    [Optional] private readonly EnabledRefRW<NeedHandleWaypointRequest> _WaypointRequestTrigger;

    private bool NeedCalcWaypoint {
        set => _WaypointRequestTrigger.ValueRW = value;
    }

    public bool IsMoveDone {
        get => _MoveData.ValueRO.isMoveDone;
        private set => _MoveData.ValueRW.isMoveDone = value;
    }

    public bool AlreadyHaveWaypoint => _WaypointBuffer.Length != 0;
    
    public float3_Q3 WaypointDestination => _WaypointBuffer[0].pos;

    public bool NeedRecalculatePath(float3 newDes) =>
        !AlreadyHaveWaypoint
     || 7 < GameHelpers.DistanceXZ_Sqr(WaypointDestination, newDes);
    
    /// <summary>
    /// This method is high-performance
    /// </summary>
    public void MoveStraightTo(float3_Q3 targetLocPos) {
        IsMoveDone       = false;
        NeedCalcWaypoint = false;

        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        _WaypointBuffer.Resize(1, NativeArrayOptions.ClearMemory);
        _WaypointBuffer.FrontRW().pos = targetLocPos;
    }
    
    /// <summary>
    /// This method is expensive because it has to run pathfinding algorithm.
    /// (Try to use it as sparingly as possible)
    /// </summary>
    public void MoveSmartTo(float3_Q3 targetLocPos) {
        IsMoveDone       = false;
        NeedCalcWaypoint = true;

        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        _WaypointBuffer.Clear();
        _WaypointRequest.ValueRW.targetLocPos = targetLocPos;
    }

    public void TeleTo(float3_Q3 targetLocPos) {
        IsMoveDone       = true;
        NeedCalcWaypoint = false;

        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        _WaypointBuffer.Clear();

        _MoveData.ValueRW.fixedPos = targetLocPos;
    }

    public void SyncFromLocTrans(in LocalTransform locTrans) {
        TeleTo(locTrans.Position.Quantizate3());

        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        _MoveData.ValueRW.targetLocDir = locTrans.Forward().Quantizate3().xz;
    }
}