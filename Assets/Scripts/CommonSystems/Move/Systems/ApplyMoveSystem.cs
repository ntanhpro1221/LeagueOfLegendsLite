using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(MoveSystemGroup))]
public partial struct ApplyMoveSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<CommonGameRulesData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new ApplyMoveJob {
            entityRotateSpeed = SystemAPI.GetSingleton<CommonGameRulesData>().entityRotateSpeed
          , fixedDeltaTime    = SystemAPI.Time.fixedDeltaTime
        }.ScheduleParallel(state.Dependency);

        state.Dependency = new StopDisabledMoveJob()
            .ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(MoveableTag))]
    [WithNone(
        typeof(NetworkDestroyedTag))]
    [WithPresent(
        typeof(RotationData.ApplyToEntity))]
    [BurstCompile]
    public partial struct ApplyMoveJob : IJobEntity {
        public float entityRotateSpeed;
        public float fixedDeltaTime;

        [BurstCompile]
        public void Execute(
            ref MoveData                             moveData
          , ref DynamicBuffer<WaypointBuffer>        waypoints
          , ref LocalTransform                       localTrans
          , ref RotationData                         rotationData
          , ref PhysicsVelocity                      velocity
          , EnabledRefRO<RotationData.ApplyToEntity> rotateEntityTrigger) {
            DoMove(ref moveData, ref waypoints,       ref localTrans,    ref rotationData
,                                rotateEntityTrigger, out var newLinear, out var newAngular);

            // APPLY VELOCITY
            GameHelpers.AssignLinearVelocity(ref velocity, newLinear, moveData.controlYAxis);
            if (rotateEntityTrigger.ValueRO) velocity.Angular = newAngular;
        }

        [BurstCompile]
        private void DoMove(
            ref MoveData                                 moveData
          , ref DynamicBuffer<WaypointBuffer>            waypoints
          , ref LocalTransform                           locTrans
          , ref RotationData                             rotationData
          , in  EnabledRefRO<RotationData.ApplyToEntity> rotateEntityTrigger
          , out float3                                   newLinear
          , out float3                                   newAngular) {
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
                    if (waypoints.IsEmpty) moveData.isMoveDone = true;
                    // Move with the remain value of disCanMove_WithoutY
                    else
                        newPos = math.lerp(
                                newPos
                              , waypoints.BackRO().pos
                              , disCanMove_WithoutY / disToTarget_WithoutY)
                            .Quantizate3();

                    // fix to new position
                    moveData.FixToPos(newPos);
                } else { // Move by Unity physics
                    // Yes, this is correct: here I use disToTarget_WithoutY
                    // Because velocity is only exactly for X and Z
                    newLinear = moveData.moveSpeed / disToTarget_WithoutY * moveVector;

                    // indicate that Unity will move this entity
                    moveData.isFixedPos = false;

                    // ROTATE RECALCULATING ONLY IF MOVING
                    rotationData.RotateTo(moveVector.Quantizate3().xz);
                }
            }

            // DO ROTATE
            newAngular = float3.zero;

            if (rotateEntityTrigger.ValueRO) {
                quaternion rotateTarget = quaternion.LookRotation(rotationData.rotation.Full, math.up());
                float      rotateVecY   = mathHelpers.EulerDiff(locTrans.Rotation, rotateTarget).y;
                float      rotateDis    = math.abs(rotateVecY);
                if (rotateDis <= entityRotateSpeed * fixedDeltaTime)
                    locTrans.Rotation = rotateTarget;
                else newAngular.y     = entityRotateSpeed / rotateDis * rotateVecY;
            }
        }
    }

    [WithAll(typeof(Simulate))]
    [WithNone(typeof(NetworkDestroyedTag))]
    [WithDisabled(typeof(MoveableTag))]
    [BurstCompile]
    public partial struct StopDisabledMoveJob : IJobEntity {
        public void Execute(
            ref PhysicsVelocity                      velocity
          , ref RotationData                         rotationData
          , in  MoveData                             moveData
          , EnabledRefRO<RotationData.ApplyToEntity> rotateEntityTrigger) {
            GameHelpers.AssignLinearVelocity(ref velocity, float3.zero, moveData.controlYAxis);
            if (rotateEntityTrigger.ValueRO) velocity.Angular = float3.zero;
        }
    }
}