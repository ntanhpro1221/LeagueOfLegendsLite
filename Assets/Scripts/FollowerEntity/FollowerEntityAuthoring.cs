using Pathfinding.ECS;
using Pathfinding.PID;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Pathfinding {
	public class FollowerEntityAuthoring : MonoBehaviour {
		[SerializeField]
		AgentCylinderShape shape = new AgentCylinderShape {
			height = 2, radius = 0.5f,
		};

		[SerializeField]
		MovementSettings movement = new MovementSettings {
			follower = new PIDMovement {
				rotationSpeed = 600, speed = 5, maxRotationSpeed = 720, maxOnSpotRotationSpeed = 720, slowdownTime = 0.5f, desiredWallDistance = 0.5f, allowRotatingOnSpot = true, leadInRadiusWhenApproachingDestination = 1f,
			}
		  , stopDistance = 0.2f, rotationSmoothing = 0f, groundMask = -1, isStopped = false,
		};

		[SerializeField]
		OrientationMode orientationBacking = OrientationMode.ZAxisForward;

		[SerializeField]
		MovementPlaneSource movementPlaneSourceBacking = MovementPlaneSource.Graph;

		[SerializeField]
		public ManagedState managedState = new ManagedState {
			enableLocalAvoidance = false
		  , pathfindingSettings  = PathRequestSettings.Default
		  , enableGravity        = false
		};

		[SerializeField]
		Pathfinding.ECS.AutoRepathPolicy autoRepathBacking = Pathfinding.ECS.AutoRepathPolicy.Default;

		public class Baker : Baker<FollowerEntityAuthoring> {
			public override void Bake(FollowerEntityAuthoring authoring) {
				Entity entity = GetEntity(TransformUsageFlags.Dynamic);
				var    pos    = authoring.transform.position;

				//Seems like no initial are values required
				AddComponent(entity, new MovementControl { });
				AddComponent(entity, new SearchState { });
				AddComponent(entity, new ResolvedMovement { });
				AddComponent(entity, new SimulateMovement { });
				AddComponent(entity, new SimulateMovementRepair { });
				AddComponent(entity, new SimulateMovementControl { });
				AddComponent(entity, new SimulateMovementFinalize { });

				//The components for follower entity to function
				AddComponent(entity, new MovementState(pos));
				AddComponent(entity, new AgentMovementPlane(authoring.transform.rotation));
				AddComponent(entity, new DestinationPoint {
					destination = new float3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity),
				});
				AddComponent(entity, authoring.autoRepathBacking);
				AddComponent(entity, authoring.movement);

				AddComponentObject(entity, authoring.managedState);
				AddComponent(entity, new ManagedStateInit { });
				AddComponent(entity, new MovementStatistics {
					estimatedVelocity = float3.zero, lastPosition = pos,
				});
				AddComponent(entity, authoring.shape);

				if (authoring.orientationBacking == OrientationMode.YAxisForward) {
					AddComponent<OrientationYAxisForward>(entity);
				}

				AddComponent(entity, new ReadyToTraverseOffMeshLink { });
				SetComponentEnabled<ReadyToTraverseOffMeshLink>(entity, false);
				AddSharedComponent(entity, new AgentMovementPlaneSource { value = authoring.movementPlaneSourceBacking });
			}
		}
	}

	//Tag to initialize ManagedState on an entity, the ManagedState should already be added to the entity
	[GhostComponent(PrefabType = GhostPrefabType.Server)]
	public struct ManagedStateInit : IComponentData { }

	//Update in TransformSystemGroup ensures that the ManagedState is setup before any simulation occurs
	[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
	[UpdateInGroup(typeof(TransformSystemGroup))]
	public partial struct ManagedStateSetupSystem : ISystem {
		[BurstCompile]
		public void OnCreate(ref SystemState state) {
			state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
			state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<
				ManagedState
			  , ManagedStateInit>().Build());
		}

		public void OnUpdate(ref SystemState state) {
			var ecb = SystemAPI
				.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
				.CreateCommandBuffer(state.WorldUnmanaged);

			foreach (var (
				managedState
			  , entity
				) in SystemAPI
				.Query<ManagedState>()
				.WithAll<ManagedStateInit>()
				.WithEntityAccess()) {
				managedState.pathTracer = new PathTracer(Allocator.Persistent);
				ecb.RemoveComponent<ManagedStateInit>(entity);
			}
		}
	}
}