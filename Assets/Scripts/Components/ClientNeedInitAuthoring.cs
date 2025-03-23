using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct ClientNeedInitTag : IComponentData { }

public class ClientNeedInitAuthoring : MonoBehaviour {
    private class Baker : Baker<ClientNeedInitAuthoring> {
        public override void Bake(ClientNeedInitAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<ClientNeedInitTag>(entity);
        }
    }
}