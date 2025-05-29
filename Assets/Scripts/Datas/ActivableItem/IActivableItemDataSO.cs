using System.Collections.Generic;
using NGDtuanh.Collections;
using Unity.Entities;
using UnityEngine;

public abstract class IActivableItemDataSO : ScriptableObject {
    public const string ASSET_PATH = "Activable Item/";

    public Sprite avatar;
    public string itemName;

    [TextArea(1, 10)] public string description;
    [TextArea(1, 5)] public string details;

    public ItemActivationCondition     activationCondition;
    public int                         maxLevel;
    public List<ItemCommonLeveledData> leveledData_Common;
    
    public abstract CovDictionary<int, List<float_Q3>> GenerateConcreteData_IntKey();
    public abstract Dictionary<string, List<float_Q3>> GenerateConcreteData_StringKey();
    public abstract List<float_Q3>                     GetConcreteLeveledData(string keyStr);

    public abstract void AddPrefabBuffer(IBaker baker, in Entity entity);
}