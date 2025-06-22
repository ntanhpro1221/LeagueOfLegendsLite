using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "RubyCrystal_ItemSO", menuName = ASSET_PATH + "RubyCrystal_ItemSO")]
public class RubyCrystal_ItemSO : ActivableItemSO_Generic<
    RubyCrystal_ItemSO.ConcreteProperty
  , RubyCrystal_ItemSO.ConcretePrefab
  , RubyCrystal_ItemSO.PrefabBuffer> {
    public enum ConcreteProperty { }

    public enum ConcretePrefab { }

    public struct PrefabBuffer : IActivableItemPrefabBuffer {
        public Entity        entity;
        Entity IActivableItemPrefabBuffer.entity { set => entity = value; }
    }
}