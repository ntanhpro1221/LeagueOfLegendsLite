using System;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(InputLocalUpdateSystemGroup))]
[UpdateAfter(typeof(PlayerInputUpdateSystem))]
public partial struct PlayerInputResetSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job().Schedule(state.Dependency);
    }

    [WithAll(
        typeof(GhostOwnerIsLocal)
      , typeof(PlayerInputResetting))]
    [BurstCompile]
    public partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(ref PlayerInputData inputData, in LocalTransform locTrans) {
            inputData = new PlayerInputData();

            // set event to notify to server
            inputData.triggers.Set(InputRequestId.DoneReset);

            // set move pos
            inputData.requestData.moveLocTarget = locTrans.Position.Quantizate3();
        }
    }
}