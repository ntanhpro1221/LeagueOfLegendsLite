using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct PrevStateIsDead : IComponentData, IEnableableComponent { }

public class PrevStateIsDeadAuthoring : MonoBehaviour {
    private class Baker : DisabledTagBaker<PrevStateIsDeadAuthoring, PrevStateIsDead> { }
}