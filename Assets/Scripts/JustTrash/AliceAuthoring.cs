using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using NGDtuanh.Collections;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.InputSystem;

public class AliceAuthoring : MonoBehaviour {
    public SerializedDictionary<TeamType, EnumMap<MinionId, GameObject>> lmao;
    public List<EnumMap<MinionId, GameObject>>                           burh;
    
    private class Baker : Baker<AliceAuthoring> {
        public override void Bake(AliceAuthoring authoring) {
        }
    }
}
