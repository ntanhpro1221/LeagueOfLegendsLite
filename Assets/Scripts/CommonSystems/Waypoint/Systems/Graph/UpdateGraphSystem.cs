using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(UpdateGraphSystemGroup))]
public partial struct UpdateGraphSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<ObstacleConfigData>(); // void run in wrong scene
    }
    
    public void OnUpdate(ref SystemState state) {
        AstarPath.active.navmeshUpdates.ForceUpdate();
        AstarPath.active.FlushGraphUpdates();
    }
}