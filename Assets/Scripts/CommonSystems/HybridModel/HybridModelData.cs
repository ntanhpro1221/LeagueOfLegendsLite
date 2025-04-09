using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct HybridModelData : ICleanupComponentData {
    public UnityObjectRef<Transform>            transformRef;
    public UnityObjectRef<SharedAnimController> animCtrlRef;
    public UnityObjectRef<Outline>              outlineRef;
    public UnityObjectRef<SkillPreviewShower>    attackRangeShowerRef;
}