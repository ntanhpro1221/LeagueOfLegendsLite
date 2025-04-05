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
    public float3_Q3                                                     hihi;
    public float_Q3                                                     huhu;
    
    private class Baker : Baker<AliceAuthoring> {
        public override void Bake(AliceAuthoring authoring) {
            // BlobArray<int> array;
            // arr
        }
    }
}
