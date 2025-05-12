#if MODULE_ENTITIES
using Unity.Entities;
using Unity.NetCode;

namespace Pathfinding.ECS {
	/// <summary>
	/// The movement plane source for an agent.
	///
	/// See: <see cref="MovementPlaneSource"/>
	/// </summary>
	[System.Serializable]
	[GhostComponent(PrefabType = GhostPrefabType.Server)]
	public struct AgentMovementPlaneSource : ISharedComponentData {
		public MovementPlaneSource value;
	}
}
#endif
