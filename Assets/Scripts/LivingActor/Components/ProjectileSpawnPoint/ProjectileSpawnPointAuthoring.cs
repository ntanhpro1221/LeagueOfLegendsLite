using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct ProjectileSpawnPoint : IComponentData {
    [GhostField] public InitTransform point;
}

public class ProjectileSpawnPointAuthoring : MonoBehaviour {
    public Transform point;

    private class Baker : ExtendBaker<ProjectileSpawnPointAuthoring> {
        public override void Bake(ProjectileSpawnPointAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent(entity, new ProjectileSpawnPoint {
                point = new InitTransform {
                    position = (authoring.point.position - authoring.transform.position)
                        .Quantizate3()
                  , rotation = (authoring.point.rotation * Quaternion.Inverse(authoring.transform.rotation))
                        .Value().Quantizate3()
                }
            });
        }
    }
}