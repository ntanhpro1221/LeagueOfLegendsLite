#if MODULE_ENTITIES
using Unity.Entities;
using Unity.NetCode;

namespace Pathfinding.ECS {
	/// <summary>Enabled if the agnet is ready to start traversing an off-mesh link</summary>
	[System.Serializable]
	[GhostComponent(PrefabType = GhostPrefabType.Server)]
	public struct ReadyToTraverseOffMeshLink : IComponentData, IEnableableComponent {
	}
}
#endif
