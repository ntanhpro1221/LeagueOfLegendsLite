using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct HybridModelData : ICleanupComponentData {
    private static readonly Color AllyHighlightColor  = Color.blue;
    private static readonly Color EnemyHighlightColor = Color.red;

    public UnityObjectRef<Transform>            transformRef;
    public UnityObjectRef<SharedAnimController> animCtrlRef;
    public UnityObjectRef<Outline>              outlineRef;
    public UnityObjectRef<RotationController>   rotateRef;
    public UnityObjectRef<IndicatorShower>      indicator;
    public UnityObjectRef<EffectBodyUI>         effectBody;

    public bool                          useFake;
    public UnityObjectRef<RectTransform> fakeTransRef;

    public void InitRealModel(in InitHybridModelClientSystem.UpdateAspect data, bool isAlly) {
        // spawn
        var model = Object.Instantiate(data.SpawnRequest.ValueRO.prefabRef.Value);

        // Link model with HybridModelData
        transformRef = model.transform;
        animCtrlRef  = model.GetComponentInChildren<SharedAnimController>();
        outlineRef   = model.GetComponentInChildren<Outline>();
        rotateRef    = model.GetComponentInChildren<RotationController>();
        indicator    = model.GetComponentInChildren<IndicatorShower>();
        effectBody   = model.GetComponentInChildren<EffectBodyUI>();

        // Set highlight color
        outlineRef.Value.OutlineColor = isAlly
            ? AllyHighlightColor
            : EnemyHighlightColor;

        // SPAWN FAKE DATA
        if (data.ChampTag.IsValid) {
            useFake      = true;
            fakeTransRef = MiniMapUI.Instance.SpawnChamp(data.ChampTag.ValueRO.id, isAlly).transform as RectTransform;
        }
    }

    public readonly void UpdateModel(in SyncHybridModelClientSystem.UpdateModelAspect data) {
        // POSITION
        if (!data.Pos.IsAnyNaN()) {
            transformRef.Value.position = data.Pos;
            if (useFake) MiniMapUI.Instance.UpdatePosInMap(fakeTransRef.Value, data.Pos.xz);
        }

        // ROTATION
        rotateRef.Value.RotateTo(data.Rot);

        // ANIMATION
        animCtrlRef.Value.SyncAnim(
            data.Anim.curAnim
          , data.Anim.currentSessionToRestart
          , data.Anim.hardCutAnim);

        // HIGHLIGHT
        bool isHighlighting = data.IsHighlighting;
        if (isHighlighting != outlineRef.Value.enabled)
            outlineRef.Value.enabled = isHighlighting;
    }

    public readonly void Destroy() {
        Object.Destroy(transformRef.Value.gameObject);

        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        if (fakeTransRef.IsValid())
            Object.Destroy(fakeTransRef.Value.gameObject);
    }
}