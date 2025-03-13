using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using NGDtuanh.Collections.EnumMap;
using NGDtuanh.Collections.PropertyWrapper;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

[ChunkSerializable]
public struct AliceTag : IComponentData {
}

// [Serializable]
// public class GrandOfBob {
//     public ParOfBob parOfBob;
// }
//
// [Serializable]
// public class ParOfBob {
//     public Bob bob;
// }
//
// [Serializable]
// public class Bob {
//     public BobChild bobChild;
// }
//
// [Serializable]
// public class BobChild {
//     public BobGrandChild grandChild;
// }
//
// [Serializable]
// public class BobGrandChild {
//     public EnumMap<ChampionId, string> map;
//     public string                      str;
//     public string[]                    strs;
// }

public class AliceAuthoring : MonoBehaviour {
    // public EnumMap<ChampionId, Bob>              set;
    // public SerializedDictionary<ChampionId, GrandOfBob> lmao;
    
    private class Baker : Baker<AliceAuthoring> {
        public override void Bake(AliceAuthoring authoring) {


            // EditorGUI.IndentLevelToWidth
        }
    }
}