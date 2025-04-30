using Unity.Entities;
using UnityEngine;

public struct ObstacleConfigData : IComponentData {
    public int radiusBonus;
}

public class ObstacleConfigAuthoring : MonoBehaviour {
    public int radiusBonus;

    private class Baker : ExtendBaker<ObstacleConfigAuthoring> {
        public override void Bake(ObstacleConfigAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent(entity, new ObstacleConfigData {
                radiusBonus = authoring.radiusBonus
            });
        }
    }
}