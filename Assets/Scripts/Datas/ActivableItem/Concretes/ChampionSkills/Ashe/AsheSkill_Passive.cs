using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "AsheSkill_Passive", menuName = ASSET_PATH + "Ashe/AsheSkill_Passive")]
public class AsheSkill_Passive : ActivableItemSO_Generic<
    AsheSkill_Passive.ConcreteProperty
  , AsheSkill_Passive.ConcretePrefab
  , AsheSkill_Passive.PrefabBuffer> {
    public enum ConcreteProperty { }

    public enum ConcretePrefab { }

    public struct PrefabBuffer : IActivableItemPrefabBuffer {
        public Entity        entity;
        Entity IActivableItemPrefabBuffer.entity { set => entity = value; }
    }
}