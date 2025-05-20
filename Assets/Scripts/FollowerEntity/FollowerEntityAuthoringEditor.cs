#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using Pathfinding.RVO;
using Pathfinding.ECS;
using System.Linq;

namespace Pathfinding {
	[CustomEditor(typeof(FollowerEntityAuthoring), true)]
	[CanEditMultipleObjects]
	public class FollowerEntityAuthoringEditor : EditorBase {
		//bool debug = false;
		bool tagPenaltiesOpen;

		protected override void OnDisable() {
			//base.OnDisable();
			EditorPrefs.SetBool("FollowerEntityAuthoring.tagPenaltiesOpen", tagPenaltiesOpen);
		}

		protected override void OnEnable() {
			//base.OnEnable();
			tagPenaltiesOpen = EditorPrefs.GetBool("FollowerEntityAuthoring.tagPenaltiesOpen", false);
		}

		public override bool RequiresConstantRepaint() {
			// When the debug inspector is open we want to update it every frame
			// as the agent can move
			return Application.isPlaying;
		}

		protected void AutoRepathInspector() {
			var mode = FindProperty("autoRepathBacking.mode");

			PropertyField(mode, "Recalculate Paths Automatically");
			if (!mode.hasMultipleDifferentValues) {
				var modeValue = (AutoRepathPolicy.Mode)mode.enumValueIndex;
				EditorGUI.indentLevel++;
				var period = FindProperty("autoRepathBacking.period");
				if (modeValue == AutoRepathPolicy.Mode.EveryNSeconds || modeValue == AutoRepathPolicy.Mode.Dynamic) {
					FloatField(period, min: 0f);
				}

				if (modeValue == AutoRepathPolicy.Mode.Dynamic) {
					EditorGUILayout.HelpBox("The path will be recalculated at least every " + period.floatValue.ToString("0.0") + " seconds, but more often if the destination changes quickly", MessageType.None);
				}

				EditorGUI.indentLevel--;
			}
		}

		void PathfindingSettingsInspector() {
			bool anyCustomTraversalProvider = this.targets.Any(s => (s as FollowerEntityAuthoring).managedState.pathfindingSettings.traversalProvider != null);
			if (anyCustomTraversalProvider) {
				EditorGUILayout.HelpBox("Custom traversal provider active", MessageType.None);
			}

			PropertyField("managedState.pathfindingSettings.graphMask", "Traversable Graphs");

			tagPenaltiesOpen = EditorGUILayout.Foldout(tagPenaltiesOpen, new GUIContent("Tags", "Settings for each tag"));
			if (tagPenaltiesOpen) {
				EditorGUI.indentLevel++;
				var traversableTags = this.targets.Select(s => (s as FollowerEntityAuthoring).managedState.pathfindingSettings.traversableTags).ToArray();
				SeekerEditor.TagsEditor(FindProperty("managedState.pathfindingSettings.tagPenalties"), traversableTags);
				for (int i = 0; i < targets.Length; i++) {
					(targets[i] as FollowerEntityAuthoring).managedState.pathfindingSettings.traversableTags = traversableTags[i];
				}

				EditorGUI.indentLevel--;
			}
		}

		protected override void Inspector() {
			Undo.RecordObjects(targets, "Modify FollowerEntityAuthoring settings");
			EditorGUI.BeginChangeCheck();
			Section("Shape");
			FloatField("shape.radius", min: 0.01f);
			FloatField("shape.height", min: 0.01f);
			Popup("orientationBacking", new[] { new GUIContent("ZAxisForward (for 3D games)"), new GUIContent("YAxisForward (for 2D games)") }, "Orientation");

			Section("Movement");
			FloatField("movement.follower.speed",         min: 0f);
			FloatField("movement.follower.rotationSpeed", min: 0f);
			var maxRotationSpeed = FindProperty("movement.follower.rotationSpeed");
			FloatField("movement.follower.maxRotationSpeed", min: maxRotationSpeed.hasMultipleDifferentValues ? 0f : maxRotationSpeed.floatValue);
			if (ByteAsToggle("movement.follower.allowRotatingOnSpotBacking", "Allow Rotating On The Spot")) {
				EditorGUI.indentLevel++;
				FloatField("movement.follower.maxOnSpotRotationSpeed",        min: 0f);
				FloatField("movement.follower.slowdownTimeWhenTurningOnSpot", min: 0f);
				EditorGUI.indentLevel--;
			}

			Slider("movement.positionSmoothing", left: 0f, right: 0.5f);
			Slider("movement.rotationSmoothing", left: 0f, right: 0.5f);
			FloatField("movement.follower.slowdownTime",                           min: 0f);
			FloatField("movement.stopDistance",                                    min: 0f);
			FloatField("movement.follower.leadInRadiusWhenApproachingDestination", min: 0f);
			FloatField("movement.follower.desiredWallDistance",                    min: 0f);

			// if (PropertyField("managedState.enableGravity", "Gravity")) {
			// 	EditorGUI.indentLevel++;
			// 	PropertyField("movement.groundMask", "Raycast Ground Mask");
			// 	EditorGUI.indentLevel--;
			// }
			var movementPlaneSource = FindProperty("movementPlaneSourceBacking");
			PropertyField(movementPlaneSource, "Movement Plane Source");
			if (AstarPath.active != null && AstarPath.active.data.graphs != null) {
				var possiblySpherical = AstarPath.active.data.navmeshGraph != null && !AstarPath.active.data.navmeshGraph.RecalculateNormals;
				if (!possiblySpherical && !movementPlaneSource.hasMultipleDifferentValues && (MovementPlaneSource)movementPlaneSource.intValue == MovementPlaneSource.Raycast) {
					EditorGUILayout.HelpBox("Using raycasts as the movement plane source is only recommended if you have a spherical or otherwise non-planar world. It has a performance overhead.", MessageType.Info);
				}

				if (!possiblySpherical && !movementPlaneSource.hasMultipleDifferentValues && (MovementPlaneSource)movementPlaneSource.intValue == MovementPlaneSource.NavmeshNormal) {
					EditorGUILayout.HelpBox("Using the navmesh normal as the movement plane source is only recommended if you have a spherical or otherwise non-planar world. It has a performance overhead.", MessageType.Info);
				}
			}

			Section("Pathfinding");
			PathfindingSettingsInspector();
			AutoRepathInspector();


			if (SectionEnableable("Local Avoidance", "managedState.enableLocalAvoidance")) {
				if (Application.isPlaying && RVOSimulator.active == null && !EditorUtility.IsPersistent(target)) {
					EditorGUILayout.HelpBox("There is no enabled RVOSimulator component in the scene. A single global RVOSimulator component is required for local avoidance.", MessageType.Warning);
				}

				FloatField("managedState.rvoSettings.agentTimeHorizon",    min: 0f, max: 20.0f);
				FloatField("managedState.rvoSettings.obstacleTimeHorizon", min: 0f, max: 20.0f);
				PropertyField("managedState.rvoSettings.maxNeighbours");
				ClampInt("managedState.rvoSettings.maxNeighbours", min: 0, max: SimulatorBurst.MaxNeighbourCount);
				PropertyField("managedState.rvoSettings.layer");
				PropertyField("managedState.rvoSettings.collidesWith");
				Slider("managedState.rvoSettings.priority", left: 0f, right: 1.0f);
				PropertyField("managedState.rvoSettings.locked");
			}

			Section("Debug");
			PropertyField("movement.debugFlags",            "Movement Debug Rendering");
			PropertyField("managedState.rvoSettings.debug", "Local Avoidance Debug Rendering");
			//DebugInspector();

			/*if (EditorGUI.EndChangeCheck())
			{
				for (int i = 0; i < targets.Length; i++)
				{
					var script = targets[i] as FollowerEntityAuthoring;
					script.SyncWithEntity();
				}
			}*/
		}
	}
}

#endif