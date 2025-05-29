using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[CreateAssetMenu(fileName = "AsheSkill_E", menuName = ASSET_PATH + "Ashe/AsheSkill_E")]
public class AsheSkill_E : ActivableItemDataSO_GenericBase<
    AsheSkill_E.ConcreteProperty
  , AsheSkill_E.ConcretePrefab
  , AsheSkill_E.PrefabBuffer> {
    public enum ConcreteProperty { }

    [GenerateIndex]
    public enum ConcretePrefab { }

    public struct PrefabBuffer : IActivableItemPrefabBuffer {
        [GhostField] public Entity        entity;
        Entity IActivableItemPrefabBuffer.entity { set => entity = value; }
    }
}