using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "HextechRocketbelt_ItemSO", menuName = ASSET_PATH + "HextechRocketbelt_ItemSO")]
public class HextechRocketbelt_ItemSO : ActivableItemSO_Generic<
    HextechRocketbelt_ItemSO.ConcreteProperty
  , HextechRocketbelt_ItemSO.ConcretePrefab
  , HextechRocketbelt_ItemSO.PrefabBuffer> {
    public enum ConcreteProperty { }

    public enum ConcretePrefab {
        Bullet
    }

    public struct PrefabBuffer : IActivableItemPrefabBuffer {
        public Entity                     entity;
        Entity IActivableItemPrefabBuffer.entity { set => entity = value; }
    }
}