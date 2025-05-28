using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct GlobalKDAData : IComponentData {
    public int blueKill;
    public int redKill;

    public void AddKill(TeamType team, int amount) {
        switch (team) {
            case TeamType.Blue: blueKill += amount; break;
            case TeamType.Red:  redKill  += amount; break;
        }
    }

    public readonly TextUpdater.GlobalKDA GenerateTextUpdater(TeamType myTeam) => myTeam switch {
        TeamType.Blue => new TextUpdater.GlobalKDA { kill = blueKill, dead = redKill }
      , TeamType.Red  => new TextUpdater.GlobalKDA { kill = redKill, dead  = blueKill }
      , _             => default
    };
}

public class GameStatisticAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<GameStatisticAuthoring> {
        public override void Bake(GameStatisticAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent<GlobalKDAData>(entity);
        }
    }
}