using System.Collections.Generic;
using NGDtuanh.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class IActivableItemDataSO : ScriptableObject {
    public const string ASSET_PATH = "Activable Item/";

    public Sprite avatar;
    public string itemName;

    [TextArea(1, 10)] public string description;
    [TextArea(1, 5)] public string details;

     public ItemActiveSettings   activeSettings;
    public  ItemActiveCondition  activeCondition;
    public  int                  maxLevel;
    public  List<float_Q3>       cooldownTime;
    public  List<ItemActiveCost> activeCost;
    
    public abstract CovDictionary<int, List<float_Q3>> GenerateConcreteData_IntKey();
    public abstract Dictionary<string, List<float_Q3>> GenerateConcreteData_StringKey();

    public abstract void AddPrefabBuffer(IBaker baker, in Entity entity);
}