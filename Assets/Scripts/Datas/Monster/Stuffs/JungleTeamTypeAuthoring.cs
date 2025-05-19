using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct JungleTeamTypeData : IComponentData {
    [GhostField] public TeamType team;
}

public class JungleTeamTypeAuthoring : MonoBehaviour {
    public TeamType team;

    private class Baker : ExtendBaker<JungleTeamTypeAuthoring> {
        public override void Bake(JungleTeamTypeAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent(entity, new JungleTeamTypeData {
                team = authoring.team
            });
        }
    }
}