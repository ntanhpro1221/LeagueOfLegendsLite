using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "FiendishCodex_ItemSO", menuName = ASSET_PATH + "FiendishCodex_ItemSO")]
public class FiendishCodex_ItemSO : ActivableItemSO_Generic<
    FiendishCodex_ItemSO.ConcreteProperty
  , FiendishCodex_ItemSO.ConcretePrefab
  , FiendishCodex_ItemSO.PrefabBuffer> {
    public enum ConcreteProperty { }

    public enum ConcretePrefab { }

    public struct PrefabBuffer : IActivableItemPrefabBuffer {
        public Entity        entity;
        Entity IActivableItemPrefabBuffer.entity { set => entity = value; }
    }
}