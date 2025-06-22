using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "GlowingMote_ItemSO", menuName = ASSET_PATH + "GlowingMote_ItemSO")]
public class GlowingMote_ItemSO : ActivableItemSO_Generic<
    GlowingMote_ItemSO.ConcreteProperty
  , GlowingMote_ItemSO.ConcretePrefab
  , GlowingMote_ItemSO.PrefabBuffer> {
    public enum ConcreteProperty { }

    public enum ConcretePrefab { }

    public struct PrefabBuffer : IActivableItemPrefabBuffer {
        public Entity        entity;
        Entity IActivableItemPrefabBuffer.entity { set => entity = value; }
    }
}