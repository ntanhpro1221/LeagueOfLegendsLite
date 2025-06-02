using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct MonsterSpawnerData : IComponentData, IEnableableComponent {
    public NetworkTick spawnTick;
}

[RequireComponent(
    typeof(MonsterTagAuthoring)
  , typeof(JungleTeamTypeAuthoring)
  , typeof(MonsterExtraAuthoring))]
[RequireComponent(
    typeof(FollowerEntityFixedPathAuthoring))]
public class MonsterSpawnerAuthoring : MonoBehaviour {
    public float         spawnTime;

    private class Baker : ExtendBaker<MonsterSpawnerAuthoring> {
        public override void Bake(MonsterSpawnerAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent(entity, new MonsterSpawnerData {
                spawnTick = TickHelpers.CalcStartTick(
                    authoring.spawnTime
                  , GameSO.TickRate)
            });
        }
    }
}