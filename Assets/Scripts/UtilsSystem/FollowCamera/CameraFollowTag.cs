using Unity.Entities;
using Unity.NetCode;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct CameraFollowTag : IComponentData, IEnableableComponent { }