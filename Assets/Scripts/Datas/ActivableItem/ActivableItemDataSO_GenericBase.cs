using System;
using System.Collections.Generic;
using NGDtuanh.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

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
    public CovEnumMap<TConcreteProperty, List<float_Q3>> concreteProp;
    public CovEnumMap<TConcretePrefab, GameObject>       concretePrefab;

    public override CovDictionary<int, List<float_Q3>> GenerateConcreteData_IntKey() {
        var result = new CovDictionary<int, List<float_Q3>>();

        foreach (var (key, value) in concreteProp)
            result.Add(Convert.ToInt32(key), value);

        return result;
    }

    public override Dictionary<string, List<float_Q3>> GenerateConcreteData_StringKey() {
        var result = new Dictionary<string, List<float_Q3>>();

        foreach (var (key, value) in concreteProp)
            result.Add(key.ToString(), value);

        return result;
    }

    public override void AddPrefabBuffer(IBaker baker, in Entity entity) {
        var prefabEnumType = typeof(TConcretePrefab);
        int prefabCount    = Enum.GetValues(prefabEnumType).Length;

        var buffer = baker.AddBuffer<TPrefabBuffer>(entity);
        buffer.ResizeUninitialized(prefabCount);

        for (int i = 0; i < prefabCount; ++i)
            buffer[i] = new TPrefabBuffer { entity = baker.GetEntity(concretePrefab[(TConcretePrefab)Enum.ToObject(prefabEnumType, i)], TransformUsageFlags.Dynamic) };
    }  

    #if UNITY_EDITOR
    [SerializeField, HideInInspector]
    private int _PrevMaxLevel;

    private void OnValidate() {
        // Fix some level-based data to corresponding length
        if (_PrevMaxLevel != maxLevel) {
            _PrevMaxLevel = maxLevel;

            cooldownTime.FixToSize(maxLevel);
            activeCost.FixToSize(maxLevel);
            foreach (var value in concreteProp.Values)
                value.FixToSize(maxLevel);
        }
    }
    #endif
}