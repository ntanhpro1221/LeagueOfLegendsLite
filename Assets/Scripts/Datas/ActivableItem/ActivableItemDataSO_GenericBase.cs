using System;
using System.Collections.Generic;
using NGDtuanh.Collections;
using Unity.Entities;
using UnityEngine;

/// <typeparam name="TConcreteProperty">Concrete data field of item, mostly be used to write item's description.</typeparam>
/// <typeparam name="TConcretePrefab">Concrete prefab type that this item needs.</typeparam>
/// <typeparam name="TPrefabBuffer">Buffer component to store all <see cref="TConcretePrefab"/> in entity.</typeparam>
public class ActivableItemDataSO_GenericBase<
    TConcreteProperty
  , TConcretePrefab
  , TPrefabBuffer> :
    IActivableItemDataSO
    where TConcreteProperty : unmanaged, Enum
    where TConcretePrefab : unmanaged, Enum
    where TPrefabBuffer : unmanaged, IActivableItemPrefabBuffer {
    public CovEnumMap<TConcreteProperty, List<float_Q3>> leveledData_Concrete;
    public CovEnumMap<TConcretePrefab, GameObject>   prefab_Concrete;

    public override CovDictionary<int, List<float_Q3>> GenerateConcreteData_IntKey() {
        var result = new CovDictionary<int, List<float_Q3>>();

        foreach (var (key, value) in leveledData_Concrete)
            result.Add(Convert.ToInt32(key), value);

        return result;
    }
    
    public override Dictionary<string, List<float_Q3>> GenerateConcreteData_StringKey() {
        var result = new Dictionary<string, List<float_Q3>>();

        foreach (var (key, value) in leveledData_Concrete)
            result.Add(key.ToString(), value);

        return result;
    }

    /// <summary>
    /// TODO: Cache this
    /// </summary>
    public override List<float_Q3> GetConcreteLeveledData(string keyStr) =>
        leveledData_Concrete[Enum.Parse<TConcreteProperty>(keyStr)];

    public override void AddPrefabBuffer(IBaker baker, in Entity entity) {
        var indexMap = EnumIndexAuthoring.GetIndexMap<TConcretePrefab>();

        var buffer = baker.AddBuffer<TPrefabBuffer>(entity);
        buffer.ResizeUninitialized(indexMap.Count);

        foreach (var (key, index) in indexMap)
            buffer[index] = new TPrefabBuffer { entity = baker.GetEntity(prefab_Concrete[key], TransformUsageFlags.Dynamic) };
    }
}