using System;
using NGDtuanh.PropertySet;
using Unity.Entities;

[Serializable]
public class AllChampionDataManaged : PropertySet<ChampionId, ChampionDataManaged>, IComponentData { }