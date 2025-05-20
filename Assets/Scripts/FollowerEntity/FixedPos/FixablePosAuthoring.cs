using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Pathfinding {
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct FixablePosData : IComponentData {
        public float3 pos;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct FixablePosTrigger : IComponentData, IEnableableComponent { }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct RVOUpdateData : IComponentData {
        public bool enable;
        public bool locked;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct RVOUpdateTrigger : IComponentData, IEnableableComponent { }

    public class FixablePosAuthoring : MonoBehaviour {
        private class Baker : ExtendBaker<FixablePosAuthoring> {
            public override void Bake(FixablePosAuthoring authoring) {
                GetDynamicEntity(out var entity);

                AddComponent<FixablePosData>(entity);
                AddComponentDisabled<FixablePosTrigger>(entity);
                AddComponentDisabled<RVOUpdateTrigger>(entity);

                // This initialization is necessary, don't try to remove it
                AddComponent(entity, new RVOUpdateData {
                    enable = true
                  , locked = false
                });
            }
        }
    }
}