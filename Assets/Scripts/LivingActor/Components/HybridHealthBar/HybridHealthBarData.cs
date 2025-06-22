using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct HybridHealthBarData : ICleanupComponentData {
    public Dynamic dynamic;
    public Sticky  sticky;

    public struct Dynamic {
        public float                         deltaY;
        public UnityObjectRef<RectTransform> transRef;
        public UnityObjectRef<HealthBarUI>   uiRef;
        public UnityObjectRef<EffectIconUI>  effectIconRef;

        public void Init(in HybridHealthBarInitRequest spawnRequest) {
            // spawn
            var healthBar = Object.Instantiate(spawnRequest.dynamicHealthBarPrefab.Value, CanvasInspector.Instance.HealthBarRoot);

            // Link healthBar with HybridHealthBarData
            deltaY        = spawnRequest.deltaY;
            transRef      = healthBar.transform as RectTransform;
            uiRef         = healthBar.GetComponent<HealthBarUI>();
            effectIconRef = healthBar.GetComponent<EffectIconUI>();
        }

        public readonly void Update(
            in HealthBarUI.UpdateData updateData
          , in LocalTransform         locTrans
          , bool                      active
          , Camera                    cam) {
            uiRef.Value.UpdateUI(updateData);
            transRef.Value.gameObject.SetActive(active);
            
            transRef.Value.position = cam!
                .WorldToScreenPoint(locTrans.Position.WithAddY(deltaY))
                .WithoutZ();
        }

        public readonly void Destroy() {
            Object.Destroy(transRef.Value.gameObject);
        }
    }

    public struct Sticky {
        public bool                             initialized;
        public UnityObjectRef<TeamStatusItemUI> uiRef;

        public void Init(bool isAlly, ChampionId champ) {
            initialized = true;

            uiRef = TeamStatusHolderUI.Instance.SpawnItem(isAlly);
            uiRef.Value.SetAvatar(champ);
        }

        public readonly void Update(
            in HealthBarUI.UpdateData  updateData
          , in DeadTriggerForUIData    deadTriggerUI
          , in NetworkTick             curTick
          , in DeadStateData           deadData
          , in EnabledRefRO<DeadState> deadState) {
            uiRef.Value.HealthBarUI.UpdateUI(updateData);
            
            deadTriggerUI.UpdateHandler(uiRef.Value.DeadHandler, in curTick, in deadData, in deadState);
        }

        public readonly void Destroy() {
            Object.Destroy(uiRef.Value.gameObject);
        }
    }
}