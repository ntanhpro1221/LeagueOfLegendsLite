#if MODULE_ENTITIES
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace Pathfinding.ECS {
	using Pathfinding;

	[GhostComponent(PrefabType = GhostPrefabType.Server)]
	public struct SearchState : IComponentData {
	}
}
#endif
