using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct KDAData : IComponentData {
    [GhostField] public int kill;
    [GhostField] public int dead;
    [GhostField] public int assist;

    public readonly TextUpdater.KDA GenerateTextUpdater() => new() {
        kill   = kill
      , dead   = dead
      , assist = assist
    };
}

public struct CreepScoreData : IComponentData {
    [GhostField] public float_Q3 creepScore;

    public readonly TextUpdater.CreepScore GenerateTextUpdater() => new() {
        creepScore = (int)creepScore
    };
}

public struct GoldData : IComponentData {
    [GhostField] public float_Q3 gold;
}

public class ChampStatisticAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<ChampStatisticAuthoring> {
        public override void Bake(ChampStatisticAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent<KDAData>(entity);
            AddComponent<CreepScoreData>(entity);
            AddComponent<GoldData>(entity);
        }
    }
}