using Unity.Entities;
using UnityEngine;

public struct TeamTypeData : IComponentData {
    public TeamType teamType;
}

public class TeamTypeAuthoring : MonoBehaviour {
    public TeamType teamType;

    private class Baker : Baker<TeamTypeAuthoring> {
        public override void Bake(TeamTypeAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TeamTypeData {
                teamType = authoring.teamType
            });
        }
    }
}