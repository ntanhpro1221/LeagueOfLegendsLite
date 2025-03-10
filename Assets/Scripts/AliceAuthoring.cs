using System;
using AYellowpaper.SerializedCollections;
using NGDtuanh.PropertySet;
using Unity.Entities;
using UnityEngine;

[ChunkSerializable]
public struct AliceTag : IComponentData {
}

[Serializable]
public class Bob {
    public string     str;
    public int        num;
    public ChampionId champ;
    public Sprite     sprite;
}

public class AliceAuthoring : MonoBehaviour {
    public AllChampionDataManaged                data;
    public SerializedDictionary<ChampionId, Bob> dict;
    public PropertySet<ChampionId, Bob>          set;
    
    private class Baker : Baker<AliceAuthoring> {
        public override void Bake(AliceAuthoring authoring) {
        }
    }
}