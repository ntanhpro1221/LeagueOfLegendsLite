using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "SampleItemSO", menuName = ASSET_PATH + "SampleItemSO")]
public class Sample_ItemSO : ActivableItemSO_Generic<
    Sample_ItemSO.ConcreteProperty
  , Sample_ItemSO.ConcretePrefab
  , Sample_ItemSO.PrefabBuffer> {
    public enum ConcreteProperty { }

    public enum ConcretePrefab { }

    public struct PrefabBuffer : IActivableItemPrefabBuffer {
        public Entity        entity;
        Entity IActivableItemPrefabBuffer.entity { set => entity = value; }
    }
}