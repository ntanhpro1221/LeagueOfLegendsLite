using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[CreateAssetMenu(fileName = "AsheSkill_Passive", menuName = ASSET_PATH + "Ashe/AsheSkill_Passive")]
public class AsheSkill_Passive : ActivableItemDataSO_GenericBase<
    AsheSkill_Passive.ConcreteProperty
  , AsheSkill_Passive.ConcretePrefab
  , AsheSkill_Passive.PrefabBuffer> {
    public enum ConcreteProperty { }

    [GenerateIndex]
    public enum ConcretePrefab { }

    public struct PrefabBuffer : IActivableItemPrefabBuffer {
        public Entity        entity;
        Entity IActivableItemPrefabBuffer.entity { set => entity = value; }
    }
}