using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public enum TeamType {
    None      = 0
  , Blue      = 1
  , Red       = 2
  , Spectator = 3
}

public struct TeamTypeData : IComponentData {
    [GhostField] public TeamType teamType;

    public TeamTypeData(TeamType teamType) {
        this.teamType = teamType;
    }

    public bool IsSameTeam(TeamTypeData other) => teamType == other.teamType;
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