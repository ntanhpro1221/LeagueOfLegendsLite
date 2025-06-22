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

public struct OwnChampBuyable : IComponentData {
    public int                      hash;
    public Strum.Items.Fields<bool> buyable;

    private bool _NeedUpdate;

    public void MarkNeedUpdate() => _NeedUpdate = true;

    public bool PopNeedUpdate() {
        var result = _NeedUpdate;
        _NeedUpdate = false;
        return result;
    }
}

public class GameStatisticAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<GameStatisticAuthoring> {
        public override void Bake(GameStatisticAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent<GlobalKDAData>(entity);
            AddComponent<OwnChampBuyable>(entity);
        }
    }
}