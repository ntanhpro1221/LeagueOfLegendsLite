using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public enum LaneType {
    None = 0
  , Top  = 1
  , Mid  = 2
  , Bot  = 3
}

public struct LaneTypeData : IComponentData {
    [GhostField] public LaneType laneType;
}

public class LaneTypeAuthoring : MonoBehaviour {
    public LaneType laneType;

    private class Baker : Baker<LaneTypeAuthoring> {
        public override void Bake(LaneTypeAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new LaneTypeData {
                laneType = authoring.laneType
            });
        }
    }
}