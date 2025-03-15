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
    protected override void OnUpdate() {
        using var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (
            networkId
          , entity) in SystemAPI
            .Query<RefRO<NetworkId>>()
            .WithNone<NetworkStreamInGame>()
            .WithEntityAccess()) {
            ecb.AddComponent<NetworkStreamInGame>(entity);
        }

        ecb.Playback(EntityManager);
    }
}
