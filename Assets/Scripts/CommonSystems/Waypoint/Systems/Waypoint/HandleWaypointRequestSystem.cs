using System.Collections.Generic;
using NGDtuanh.Utils;
using Pathfinding;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(UpdateWaypointSystemGroup))]
[UpdateAfter(typeof(FixWaypointSystem))]
public partial struct HandleWaypointRequestSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<NeedHandleWaypointRequest>();
    }

    public void OnUpdate(ref SystemState state) {
        List<ABPath> pendingPath = Pathfinding.Pooling.ListPool<ABPath>.Claim();

        HandleAllRequestImmediately(ref state, pendingPath);
        ReturnRequestResult(ref state, pendingPath);
        CleanUpPendingPathList(pendingPath);

        Pathfinding.Pooling.ListPool<ABPath>.Release(ref pendingPath);
    }

    private void HandleAllRequestImmediately(ref SystemState state, List<ABPath> pendingPath) {
        // Push all requests to AstarPath
        foreach (var (
            request
          , locTrans) in SystemAPI
            .Query<
                RefRW<WaypointRequestData>
              , RefRO<LocalTransform>>()
            .WithAll<
                Simulate
              , NeedHandleWaypointRequest>()) {

            // Create new path request
            var path = ABPath.Construct(locTrans.ValueRO.Position, request.ValueRO.targetLocPos);
            path.nnConstraint.distanceMetric = DistanceMetric.ClosestAsSeenFromAbove(math.up());

            // Mark this path
            path.Claim(pendingPath);

            // Add to the pending path list
            pendingPath.Add(path);
            request.ValueRW.tmpPathId = pendingPath.LastElementIndex();

            // Push request
            AstarPath.StartPath(path);
        }

        // Wait all requests to be completed (all of them may be calculated in multi-thread?)
        // Then apply modifier
        var modifier = PathModifierHub.Raycast;
        foreach (var path in pendingPath) {
            AstarPath.BlockUntilCalculated(path);

            path.vectorPath.Add(path.endPoint);
            path.vectorPath.Reverse();
            path.vectorPath.Add(path.startPoint);

            modifier.Apply(path);
        }
    }

    private void ReturnRequestResult(ref SystemState state, List<ABPath> pendingPath) {
        foreach (var (
            request
          , waypoints) in SystemAPI
            .Query<
                RefRO<WaypointRequestData>
              , DynamicBuffer<WaypointBuffer>>()
            .WithAll<
                Simulate
              , NeedHandleWaypointRequest>()) {
            // Copy result to WaypointBuffer (In reversed order)
            waypoints.Clear();
            foreach (var point in pendingPath[request.ValueRO.tmpPathId].vectorPath)
                waypoints.Add(new WaypointBuffer(point.Quantizate3()));
        }
    }

    private void CleanUpPendingPathList(List<ABPath> pendingPath) {
        // Release path reference
        for (int i = 0; i < pendingPath.Count; ++i) {
            pendingPath[i].Release(pendingPath);
            pendingPath[i] = null;
        }

        // Resize the pending path list
        pendingPath.Clear();
    }
}