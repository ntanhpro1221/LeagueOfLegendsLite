using System.Collections.Generic;
// using BlobAssetExtend;
// using NGDtuanh.Collections.EnumMap;
using NGDtuanh.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.InputSystem;

public struct TestingTag : IComponentData { }

[GhostComponent(
    // PrefabType = GhostPrefabType.Client
  // , OwnerSendType = SendToOwnerType.All
    )]
public struct AliceTag : IInputComponentData {
    public FixedString128Bytes syncString;
}

public class AliceAuthoring : MonoBehaviour {
    private class Baker : Baker<AliceAuthoring> {
        public override void Bake(AliceAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<TestingTag>(entity);
            AddComponent<AliceTag>(entity);
        }
    }
}

public partial class AliceSystem : SystemBase {
    protected override void OnUpdate() {
        using var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        if (Keyboard.current.shiftKey.isPressed) {
            if (World.IsClient() && Keyboard.current.enterKey.wasReleasedThisFrame) {
                Debug.Log("Client shift clicked");
                foreach (var (alice, entity) in SystemAPI.Query<RefRO<TestingTag>>().WithEntityAccess())
                    ecb.RemoveComponent<AliceTag>(entity);
            }
            else if (World.IsServer() && Keyboard.current.spaceKey.wasReleasedThisFrame) {
                Debug.Log("Server shift clicked");
                foreach (var (alice, entity) in SystemAPI.Query<RefRO<TestingTag>>().WithEntityAccess())
                    ecb.RemoveComponent<AliceTag>(entity);
            }
        }
        else {
            if (World.IsClient() && Keyboard.current.enterKey.wasReleasedThisFrame) {
                Debug.Log("Client clicked");
                foreach (var (alice, entity) in SystemAPI.Query<RefRO<TestingTag>>().WithEntityAccess())
                    ecb.AddComponent<AliceTag>(entity);
            }
            else if (World.IsServer() && Keyboard.current.spaceKey.wasReleasedThisFrame) {
                Debug.Log("Server clicked");
                foreach (var (alice, entity) in SystemAPI.Query<RefRO<TestingTag>>().WithEntityAccess())
                    ecb.AddComponent<AliceTag>(entity);
            }
        }

        ecb.Playback(EntityManager);
    }
}


public partial class MakeInGameSystem : SystemBase {
    protected override void OnUpdate() { }

    void Lmao(ref BlobBuilder builder) {
        // BubleMap<
        //     int
        //   , BubleArray<BubleInt, int>
        //   , IReadOnlyCollection<int>> burh;
        // BlobAssetReference<
        //     BubleMap<int, BubleMap<int, BubleArray<BubleInt, int>
        //           , IReadOnlyCollection<int>
        //         >
        //       , ICovKVPCollection<int, IReadOnlyCollection<int>>
        //     >
        // > map;
        //
        // CovDictionary<int, CovDictionary<int, List<int>>> source = default;
        // source.CreateBlobAssetReference(ref builder, out map, null); 
    }
}
