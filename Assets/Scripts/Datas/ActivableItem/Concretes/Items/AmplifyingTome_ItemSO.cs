using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "AmplifyingTome_ItemSO", menuName = ASSET_PATH + "AmplifyingTome_ItemSO")]
public class AmplifyingTome_ItemSO : ActivableItemSO_Generic<
    AmplifyingTome_ItemSO.ConcreteProperty
  , AmplifyingTome_ItemSO.ConcretePrefab
  , AmplifyingTome_ItemSO.PrefabBuffer> {
    public enum ConcreteProperty { }

    public enum ConcretePrefab { }

    public struct PrefabBuffer : IActivableItemPrefabBuffer {
        public Entity        entity;
        Entity IActivableItemPrefabBuffer.entity { set => entity = value; }
    }
}