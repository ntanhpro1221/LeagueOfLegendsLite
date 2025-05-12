using Unity.Entities;
using UnityEngine;

namespace Pathfinding {
	public struct AutoFollowTarget_FollowerEntity : IComponentData, IEnableableComponent { }

	public class AutoFollowTarget_FollowerEntityAuthoring : MonoBehaviour {
		private class Baker : DisabledTagBaker<AutoFollowTarget_FollowerEntityAuthoring, AutoFollowTarget_FollowerEntity> { }
	}
}