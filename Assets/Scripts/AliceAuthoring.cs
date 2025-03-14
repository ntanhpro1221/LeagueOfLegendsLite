using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using NGDtuanh.Collections.EnumMap;
using NGDtuanh.Collections.PropertyWrapper;
using Unity.Entities;
using Unity.Physics.Systems;
using UnityEditor;
using UnityEngine;

[ChunkSerializable]
public struct AliceTag : IComponentData {
}

namespace NGDtuanh.Entities.StateMachine { }

public class AliceAuthoring : MonoBehaviour {
    // public EnumMap<ChampionId, Bob>              set;
    // public SerializedDictionary<ChampionId, GrandOfBob> lmao;
    
    private class Baker : Baker<AliceAuthoring> {
        public override void Bake(AliceAuthoring authoring) {
            // AddComponent(ett, );
            // PhysicsSystemGroup


            // EditorGUI.IndentLevelToWidth
        }
    }
}