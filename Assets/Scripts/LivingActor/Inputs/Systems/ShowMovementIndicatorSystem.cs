using Unity.Entities;

[UpdateInGroup(typeof(PlayerInputUpdateSystemGroup))]
public partial struct ShowMovementIndicatorSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<InputCastData>();
        state.RequireForUpdate<InputDirtyData>();
    }

    public void OnUpdate(ref SystemState state) {
        var dirtyData = SystemAPI.GetSingleton<InputDirtyData>();
        var castData  = SystemAPI.GetSingleton<InputCastData>();

        if (!PlayerInputUpdateSystem.CheckMoveEvent(dirtyData, castData)) return;

        MovementIndicatorPoolingManager.Pool(castData.groundPos);
    }
}