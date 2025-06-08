using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[CreateAssetMenu(fileName = "SampleItemDataSO", menuName = ASSET_PATH + "SampleItemDataSO")]
public class SampleItemDataSO : ActivableItemDataSO_GenericBase<
    SampleItemDataSO.ConcreteProperty
  , SampleItemDataSO.ConcretePrefab
  , SampleItemDataSO.PrefabBuffer> {
    public enum ConcreteProperty { }

    public enum ConcretePrefab { }

    public struct PrefabBuffer : IActivableItemPrefabBuffer {
        public Entity        entity;
        Entity IActivableItemPrefabBuffer.entity { set => entity = value; }
    }
}