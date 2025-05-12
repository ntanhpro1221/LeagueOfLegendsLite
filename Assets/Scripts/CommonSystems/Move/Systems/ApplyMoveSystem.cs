using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(MoveSystemGroup))]
public partial struct ApplyMoveSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<CommonGameRulesData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new ApplyMoveJob {
            rotateSpeed    = SystemAPI.GetSingleton<CommonGameRulesData>().rotateSpeed
          , fixedDeltaTime = SystemAPI.Time.fixedDeltaTime
        }.ScheduleParallel(state.Dependency);

        state.Dependency = new StopDisabledMoveJob()
            .ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(MoveableTag))]
    [WithNone(typeof(NetworkDestroyedTag))]
    [BurstCompile]
    public partial struct ApplyMoveJob : IJobEntity {
        public float rotateSpeed;
        public float fixedDeltaTime;

        [BurstCompile]
        public void Execute(
            ref MoveData                      moveData
          , ref DynamicBuffer<WaypointBuffer> waypoints
          , ref LocalTransform                localTrans
          , ref PhysicsVelocity               velocity) {
            DoMoveAndRotate(ref moveData, ref waypoints, ref localTrans, out var newLinear, out var newAngular);

            // APPLY VELOCITY
            GameHelpers.AssignLinearVelocity(ref velocity, newLinear, moveData.controlYAxis);
            velocity.Angular = newAngular;
        }

        [BurstCompile]
        private void DoMoveAndRotate(
            ref MoveData                      moveData
          , ref DynamicBuffer<WaypointBuffer> waypoints
          , ref LocalTransform                locTrans
          , out float3                        newLinear
          , out float3                        newAngular) {
            // DO MOVE
            newLinear = float3.zero;

            if (!moveData.isMoveDone && !waypoints.IsEmpty) {
                float3 moveVector           = waypoints.BackRO().pos - locTrans.Position;
                float  disToTarget_WithoutY = math.length(moveVector.WithoutY());
                float  disCanMove_WithoutY  = moveData.moveSpeed * fixedDeltaTime;

                // Manually move
                if (disToTarget_WithoutY <= disCanMove_WithoutY) {
                    float3_Q3 newPos;
                    do {
                        // Reach waypoint and remove it
                        newPos = waypoints.PopBack().pos;
                        if (waypoints.Length == 0) break;

                        // decrease distance can move
                        disCanMove_WithoutY -= disToTarget_WithoutY;

                        // recalculate distance to next waypoint
                        disToTarget_WithoutY = math.length(
                            ((float3)(waypoints.BackRO().pos - newPos))
                            .WithoutY());
                    } while (disToTarget_WithoutY <= disCanMove_WithoutY);

                    // No waypoint left => done move
                    if (waypoints.Empty()) moveData.isMoveDone = true;
                    // Move with the remain value of disCanMove_WithoutY
                    else
                        newPos = math.lerp(
                                newPos
                              , waypoints.BackRO().pos
                              , disCanMove_WithoutY / disToTarget_WithoutY)
                            .Quantizate3();

                    // fix to new position
                    moveData.FixToPos(newPos);
                }
                // Move by Unity physics
                else {
                    // Yes, this is correct: here I use disToTarget_WithoutY
                    // Because velocity is only exactly for X and Z
                    newLinear = moveData.moveSpeed / disToTarget_WithoutY * moveVector;

                    // indicate that Unity will move this entity
                    moveData.isFixedPos = false;

                    // ROTATE RECALCULATING ONLY IF MOVING
                    moveData.RotateTo(moveVector.Quantizate3().xz);
                }
            }

            // DO ROTATE
            newAngular = float3.zero;

            quaternion rotateTarget = quaternion.LookRotation(moveData.targetLocDir.Full, math.up());
            float      rotateVecY   = mathHelpers.EulerDiff(locTrans.Rotation, rotateTarget).y;
            float      rotateDis    = math.abs(rotateVecY);
            if (rotateDis <= rotateSpeed * fixedDeltaTime)
                locTrans.Rotation = rotateTarget;
            else newAngular.y     = rotateSpeed / rotateDis * rotateVecY;
        }
    }

    [WithAll(typeof(Simulate))]
    [WithNone(typeof(NetworkDestroyedTag))]
    [WithDisabled(typeof(MoveableTag))]
    [BurstCompile]
    public partial struct StopDisabledMoveJob : IJobEntity {
        public void Execute(ref PhysicsVelocity velocity, in MoveData moveData) {
            GameHelpers.AssignLinearVelocity(ref velocity, float3.zero, moveData.controlYAxis);
            velocity.Angular = float3.zero;
        }
    }
}