using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[CreateAssetMenu(fileName = "AsheSkill_W", menuName = ASSET_PATH + "Ashe/AsheSkill_W")]
public class AsheSkill_W : ActivableItemDataSO_GenericBase<
    AsheSkill_W.ConcreteProperty
  , AsheSkill_W.ConcretePrefab
  , AsheSkill_W.PrefabBuffer> {
    public enum ConcreteProperty {
        damage
      , arrowAmount
    }

    public enum ConcretePrefab {
        arrow
    }

    public struct PrefabBuffer : IActivableItemPrefabBuffer {
        public Entity        entity;
        Entity IActivableItemPrefabBuffer.entity { set => entity = value; }
    }
}