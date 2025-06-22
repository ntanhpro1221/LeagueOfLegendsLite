using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

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
            dirtyData = SystemAPI.GetSingleton<InputDirtyData>()
          , castData  = SystemAPI.GetSingleton<InputCastData>()
        }.Schedule(state.Dependency);
    }

    [WithAll(typeof(GhostOwnerIsLocal))]
    [BurstCompile]
    public partial struct Job : IJobEntity {
        public InputDirtyData dirtyData;
        public InputCastData  castData;

        [BurstCompile]
        public void Execute(ref PlayerInputData inputData, in LocalTransform locTrans) {
            // RESET EVENT
            inputData.ResetAllEvents();

            // FIRST OF ALL: CHECK COMMON REQUEST
            foreach (var request in Strum.InputRequest.Indexes)
                if (dirtyData.requestTrigger[request])
                    inputData.triggers.Set(request);
            inputData.requestData = dirtyData.requestData;

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

            // ACTIVE ITEM
            inputData.inputForActivableItem.UpdateAll(castData, dirtyData, locTrans);

            inputData.curCondition.UpdateAll(castData);

            foreach (var key in Strum.SlotItem.Indexes)
                if (dirtyData.activableItem[key].WasReleasedThisFrame())
                    inputData.triggers.Set(key);
        }
    }

    public static bool CheckMoveEvent(in InputDirtyData dirtyData, in InputCastData castData) =>
        castData.isHitWalkableGround
     && dirtyData.mouse_right.WasPressedThisFrame();

    public static bool CheckMoveAttackEvent(in InputDirtyData dirtyData, in InputCastData castData) =>
        castData.isHitWalkableGround
     && dirtyData.mouse_left.WasPressedThisFrame();
}