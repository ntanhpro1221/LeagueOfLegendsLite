using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial class ActorStateItemCommon {
    partial struct Enter {
        private void HextechRocketbelt(ref CommonUpdateData common) {
            const int DASH_SPEED = 2750;

            ref readonly var data = ref common.aspect;

            // DASH
            data.moveRequester.OverrideSpeed(DASH_SPEED);
            var dir = data.StateData.input.direction.Full;
            data.moveRequester.MoveStraightTo(
                // This is lazy normalizing (we just need a far target point).
                // This order prevent result (target point) from being overflowed.
                dir * (1e4f * float_Q3.MULTIPLIER / math.max(math.abs(dir.x), math.abs(dir.z)))
              + data.LocTrans.Position.Quantizate3());
        }
    }

    partial struct Update {
        private void HextechRocketbelt(ref CommonUpdateData common, ref SystemState state) {
            ref readonly var data = ref common.aspect;
            ref var          ecb  = ref common.ecb;

            if (!data.StateData.performData.IsReadyToPerform(common.curTick)) return;
            
            data.StateData.performData.MarkPerformed();
            
            // STOP DASH
            data.MoveRequester.DisableOverrideSpeed();
            data.MoveRequester.TeleTo(data.Position.Quantizate3());

            if (!common.isFirstTimeFull) return;
            
            // CONSTANT FOR SPAWNING BULLET
            const int   AMOUNT           = 7;
            const float ANGLE_DIS_RAD    = 20 * Mathf.Deg2Rad;
            const float DISTANCE         = 275;
            const float START_LERP_RATIO = 20 / DISTANCE;
            const int   DAMAGE           = 100;

            // SPAWN BULLET
            var direction = quaternion.LookRotation(data.StateData.input.direction.Full, math.up());
            var spawnPoint = LocalTransform.FromPositionRotation(data.Position, direction)
                .TransformPoint(data.ProjectileSpawnPoint.position);

            var prefab = SystemAPI
                .GetSingletonBuffer<HextechRocketbelt_ItemSO.PrefabBuffer>(isReadOnly: true)
                [(int)HextechRocketbelt_ItemSO.ConcretePrefab.Bullet].entity;

            var curDelRad = ANGLE_DIS_RAD * ((float)AMOUNT - 1) / 2;
            for (int i = 0; i < AMOUNT; ++i, curDelRad -= ANGLE_DIS_RAD) {
                var arrow         = ecb.Instantiate(prefab);
                var arrowRotation = math.mul(quaternion.RotateY(curDelRad), direction);
                var toEndPointVec = DISTANCE * math.normalize(arrowRotation.Forward().xz);
                var destination   = spawnPoint.Quantizate3() + new float3_Q3(toEndPointVec.x, 0, toEndPointVec.y);

                ecb.SetComponent(arrow, LocalTransform.FromPositionRotation(math.lerp(spawnPoint, destination, START_LERP_RATIO), arrowRotation));

                ecb.SetComponent(arrow, new DestroyAtDestination { destination = destination });

                ecb.SetComponent(arrow, new DamageTriggerSource {
                    damage       = DAMAGE
                  , source       = data.Entity
                  , sourcePos    = data.Position.Quantizate3()
                  , sourceScaler = data.PersonalConstructor.Construct()
                });

                ecb.SetComponent(arrow, data.Team);

                MoveRequesterAspect.MoveStraightTo(ref ecb, arrow, destination);
            }
        }
    }
}