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
    [GhostField] public TeamType team;

    public TeamTypeData(TeamType teamType) {
        this.team = teamType;
    }

    public bool IsSameTeam(TeamTypeData other) => team == other.team;

    /// <summary>
    /// Regardless who is red or who is blue
    /// </summary>
    public bool IsRedBlue(TeamTypeData other) =>
        (team == TeamType.Blue && other.team == TeamType.Red)
     || (team == TeamType.Red  && other.team == TeamType.Blue);

    public static implicit operator TeamTypeData(TeamType teamType) => new() { team = teamType };
}

public class TeamTypeAuthoring : MonoBehaviour {
    public TeamType teamType;

    private class Baker : Baker<TeamTypeAuthoring> {
        public override void Bake(TeamTypeAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TeamTypeData {
                team = authoring.teamType
            });
        }
    }
}