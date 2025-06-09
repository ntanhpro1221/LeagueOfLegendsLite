using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(InputLocalUpdateSystemGroup))]
public partial struct PlayerInputUpdateSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<InputDirtyData>();
        state.RequireForUpdate<InputCastData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job {
            dirtyBuffer = SystemAPI.GetSingletonBuffer<InputDirtyData.ActivableItemBuffer>(isReadOnly: true)
          , dirtyData = SystemAPI.GetSingleton<InputDirtyData>()
          , castData  = SystemAPI.GetSingleton<InputCastData>()
        }.Schedule(state.Dependency);
    }

    [WithAll(typeof(GhostOwnerIsLocal))]
    [BurstCompile]
    public partial struct Job : IJobEntity {
        [ReadOnly] public DynamicBuffer<InputDirtyData.ActivableItemBuffer> dirtyBuffer;

        public InputDirtyData dirtyData;
        public InputCastData  castData;

        [BurstCompile]
        public void Execute(ref PlayerInputData inputData, in LocalTransform locTrans) {
            // RESET EVENT
            inputData.ResetAllEvents();

            // CHECK UPDATE SKILL
            if (dirtyData.haveSkillUpgradeRequest)
                inputData.SetUpdateSkill(dirtyData.skillToUpgrade);
            
            // CHECK MOVE
            if (CheckMoveEvent(dirtyData, castData)) {
                inputData.SetMove(castData.walkableGroundPos);
                inputData.CancelAttack();
            }

            // CHECK ATTACK
            if (castData.isHitActor && dirtyData.mouse_left.WasPressedThisFrame()) {
                inputData.SetAttack(castData.actor);
                inputData.CancelMove();
            } else if (castData.isHitClosestEntityAtGroundHit && dirtyData.mouse_left.WasPressedThisFrame()) {
                inputData.SetAttack(castData.closestEntityAtGroundHit);
                inputData.CancelMove();
            }

            // CANCEL MOVE AND ATTACK
            if (dirtyData.key_s.WasPressedThisFrame()) {
                inputData.CancelMove();
                inputData.CancelAttack();
            }

            inputData.inputForActivableItem.UpdateAll(castData, dirtyData, locTrans);

            inputData.curCondition.UpdateAll(castData);

            for (int i = 0; i < PlayerTrigger.ITEM_COUNT; ++i)
                if (dirtyBuffer[i].key.WasReleasedThisFrame())
                    inputData.triggers.Set((PlayerTrigger.Item)i);
        }
    }

    [BurstCompile]
    public static bool CheckMoveEvent(in InputDirtyData dirtyData, in InputCastData castData) =>
        castData.isHitWalkableGround
     && dirtyData.mouse_right.WasPressedThisFrame();

    [BurstCompile]
    public static bool CheckMoveAttackEvent(in InputDirtyData dirtyData, in InputCastData castData) =>
        castData.isHitWalkableGround
     && dirtyData.mouse_left.WasPressedThisFrame();
}