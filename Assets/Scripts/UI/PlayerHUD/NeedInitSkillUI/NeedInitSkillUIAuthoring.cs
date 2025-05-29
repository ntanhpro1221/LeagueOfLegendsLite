using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct NeedInitSkillUI : IComponentData {}

public class NeedInitSkillUIAuthoring : MonoBehaviour {
    private class Baker : TagBaker<NeedInitSkillUIAuthoring, NeedInitSkillUI> { }
}