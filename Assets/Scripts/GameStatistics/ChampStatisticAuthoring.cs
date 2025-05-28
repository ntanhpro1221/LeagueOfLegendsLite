using Unity.Entities;
using UnityEngine;

public struct KDAData : IComponentData {
    public int kill;
    public int dead;
    public int assist;

    public readonly TextUpdater.KDA GenerateTextUpdater() => new() {
        kill   = kill
      , dead   = dead
      , assist = assist
    };
}

public struct CreepScoreData : IComponentData {
    public int creepScore;

    public readonly TextUpdater.CreepScore GenerateTextUpdater() => new() {
        creepScore = creepScore
    };
}

public class ChampStatisticAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<ChampStatisticAuthoring> {
        public override void Bake(ChampStatisticAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent<KDAData>(entity);
            AddComponent<CreepScoreData>(entity);
        }
    }
}