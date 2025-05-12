using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(InputLocalUpdateSystemGroup))]
public partial struct ShowMovementIndicatorSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<InputCastData>();
        state.RequireForUpdate<InputDirtyData>();
    }

    public void OnUpdate(ref SystemState state) {
        var dirtyData = SystemAPI.GetSingleton<InputDirtyData>();
        var castData  = SystemAPI.GetSingleton<InputCastData>();

        if (PlayerInputUpdateSystem.CheckMoveEvent(dirtyData, castData))
            MovementIndicatorPoolingManager.Pool(castData.groundPos, MovementIndicatorType.Green);

        if (PlayerInputUpdateSystem.CheckMoveAttackEvent(dirtyData, castData))
            MovementIndicatorPoolingManager.Pool(castData.groundPos, MovementIndicatorType.Red);
    }
}