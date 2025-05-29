using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[CreateAssetMenu(fileName = "AsheSkill_Q", menuName = ASSET_PATH + "Ashe/AsheSkill_Q")]
public class AsheSkill_Q : ActivableItemDataSO_GenericBase<
    AsheSkill_Q.ConcreteProperty
  , AsheSkill_Q.ConcretePrefab
  , AsheSkill_Q.PrefabBuffer> {
    public enum ConcreteProperty {
        attackSpeed
      , damagePerArrow
    }

    [GenerateIndex]
    public enum ConcretePrefab { }

    public struct PrefabBuffer : IActivableItemPrefabBuffer {
        [GhostField] public Entity        entity;
        Entity IActivableItemPrefabBuffer.entity { set => entity = value; }
    }
}