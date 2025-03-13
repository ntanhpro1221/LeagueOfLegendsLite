using System;
using NGDtuanh.Collections.EnumMap;
using Unity.Entities;

[Serializable]
public class AllChampionDataManaged : EnumMap<ChampionId, ChampionDataManaged>, IComponentData { }