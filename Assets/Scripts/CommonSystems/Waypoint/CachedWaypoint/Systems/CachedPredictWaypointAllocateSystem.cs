using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial struct CachedPredictWaypointAllocateSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NeedInitCachedPathAndLineCast>();
    }

    public void OnUpdate(ref SystemState state) {
        var entity = SystemAPI.GetSingletonEntity<NeedInitCachedPathAndLineCast>();

        var entityMan = state.EntityManager;

        entityMan.AddComponentObject(entity, new CachedPathData());
        entityMan.AddComponentObject(entity, new HandlingPathData());
        entityMan.AddComponentObject(entity, new CachedLineCastData());

        // Mark allocated
        entityMan.RemoveComponent<NeedInitCachedPathAndLineCast>(entity);
    }
}