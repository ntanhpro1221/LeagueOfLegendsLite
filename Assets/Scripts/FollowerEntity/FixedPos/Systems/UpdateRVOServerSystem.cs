using Pathfinding.ECS;
using Unity.Entities;
using UnityEngine;

namespace Pathfinding {
    [UpdateInGroup(typeof(AIMovementSystemGroup), OrderFirst = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct UpdateRVOServerSystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            foreach (var (
                managedState
              , updateTrigger
              , updateData
                ) in SystemAPI
                .Query<
                    ManagedState
                  , EnabledRefRW<RVOUpdateTrigger>
                  , RefRO<RVOUpdateData>>()) {
                managedState.rvoSettings.locked   = updateData.ValueRO.locked;
                managedState.enableLocalAvoidance = updateData.ValueRO.enable;
                updateTrigger.ValueRW             = false;
            }
        }
    }
}