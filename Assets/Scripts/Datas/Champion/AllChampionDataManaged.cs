using System;
using NGDtuanh.Collections;
using Unity.Entities;

[Serializable]
public class AllChampionDataManaged : CovEnumMap<ChampionId, ChampionDataManaged>, IComponentData { }