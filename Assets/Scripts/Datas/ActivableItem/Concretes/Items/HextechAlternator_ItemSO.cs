using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "HextechAlternator_ItemSO", menuName = ASSET_PATH + "HextechAlternator_ItemSO")]
public class HextechAlternator_ItemSO : ActivableItemSO_Generic<
    HextechAlternator_ItemSO.ConcreteProperty
  , HextechAlternator_ItemSO.ConcretePrefab
  , HextechAlternator_ItemSO.PrefabBuffer> {
    public enum ConcreteProperty { }

    public enum ConcretePrefab { }

    public struct PrefabBuffer : IActivableItemPrefabBuffer {
        public Entity        entity;
        Entity IActivableItemPrefabBuffer.entity { set => entity = value; }
    }
}