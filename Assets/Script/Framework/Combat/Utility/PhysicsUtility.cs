using UnityEngine;

namespace Framework.Combat.Runtime{
	public class PhysicsUtility{
		public static Transform LineOfSight(Transform transform, Vector3 positionOffset, GameObject targetObject,
			Vector3 targetOffset, int ignoreLayerMask, bool drawDebugRay){
			Transform hitTransform = null;
			RaycastHit hit;
			if (Physics.Linecast(transform.TransformPoint(positionOffset),
				    targetObject.transform.TransformPoint(targetOffset), out hit, ~ignoreLayerMask,
				    QueryTriggerInteraction.Ignore)){
				hitTransform = hit.transform;
			}
			return hitTransform;
		}

		public static void DrawLineOfSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle,
			float angleOffset, float viewDistance, bool usePhysics2D){
#if UNITY_EDITOR
			var oldColor = UnityEditor.Handles.color;
			var color = Color.yellow;
			color.a = 0.1f;
			UnityEditor.Handles.color = color;
			var halfFOV = fieldOfViewAngle * 0.5f + angleOffset;
			var beginDirection = Quaternion.AngleAxis(-halfFOV, (usePhysics2D ? transform.forward : transform.up)) *
				(usePhysics2D ? transform.up : transform.forward);
			UnityEditor.Handles.DrawSolidArc(transform.TransformPoint(positionOffset),
				(usePhysics2D ? transform.forward : transform.up), beginDirection, fieldOfViewAngle, viewDistance);
			UnityEditor.Handles.color = oldColor;
#endif
		}
	}
}
