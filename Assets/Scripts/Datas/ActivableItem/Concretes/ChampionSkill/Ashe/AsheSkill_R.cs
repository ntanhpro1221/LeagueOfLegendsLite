using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[CreateAssetMenu(fileName = "AsheSkill_R", menuName = ASSET_PATH + "Ashe/AsheSkill_R")]
public class AsheSkill_R : ActivableItemDataSO_GenericBase<
    AsheSkill_R.ConcreteProperty
  , AsheSkill_R.ConcretePrefab
  , AsheSkill_R.PrefabBuffer> {
    public enum ConcreteProperty {
        magicDamage
      , reducedDamage
    }

    [GenerateIndex]
    public enum ConcretePrefab {
        arrow  
    }

    public struct PrefabBuffer : IActivableItemPrefabBuffer {
        public Entity        entity;
        Entity IActivableItemPrefabBuffer.entity { set => entity = value; }
    }
}