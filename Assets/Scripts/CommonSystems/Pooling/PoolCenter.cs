using NGDtuanh.Singleton;
using UnityEngine;

public class PoolCenter : SceneSingleton<PoolCenter> {
    [field: SerializeField] public Pool<TeamType, MinionId> Minion    { get; private set; } = new();
    [field: SerializeField] public Pool<HealthBarId>        HealthBar { get; private set; } = new();
}