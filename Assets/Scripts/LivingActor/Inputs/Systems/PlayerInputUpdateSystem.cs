using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(InputLocalUpdateSystemGroup))]
public partial struct PlayerInputUpdateSystem : ISystem {
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

            // CHECK MOVE
            if (CheckMoveEvent(dirtyData, castData)) {
                inputData.SetMove(castData.walkableGroundPos);
                inputData.CancelAttack();
            }

            // CHECK ATTACK
            if (castData.isHitActor)
                if ( // Left click
                    dirtyData.leftMouse.WasPressedThisFrame()
                    // Release A_Key
                 || dirtyData.a_key.WasReleasedThisFrame()) {
                    inputData.SetAttack(castData.actor);
                    inputData.CancelMove(locTrans);
                }

            // CANCEL MOVE AND ATTACK
            if (dirtyData.s_key.WasPressedThisFrame()) {
                inputData.CancelMove(locTrans);
                inputData.CancelAttack();
            }
        }
    }

    [BurstCompile]
    public static bool CheckMoveEvent(in InputDirtyData dirtyData, in InputCastData castData) =>
        castData.isHitWalkableGround
     && dirtyData.rightMouse.WasPressedThisFrame();
}