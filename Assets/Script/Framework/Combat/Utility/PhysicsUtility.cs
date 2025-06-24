using UnityEngine;

namespace Framework.Combat.Runtime{
	public class PhysicsUtility{
		public static GameObject WithinSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle,
			float viewDistance, Collider[] overlapColliders, LayerMask objectLayerMask, Vector3 targetOffset,
			LayerMask ignoreLayerMask, bool drawDebugRay){
			GameObject objectFound = null;
			var hitCount = Physics.OverlapSphereNonAlloc(transform.TransformPoint(positionOffset), viewDistance,
				overlapColliders, objectLayerMask, QueryTriggerInteraction.Ignore);
			if (hitCount > 0){
#if UNITY_EDITOR
				if (hitCount >= overlapColliders.Length){
					Debug.LogWarning(
						"命中数大于等于传入的数组。这将导致碰撞对象丢失。");
				}
#endif
				float minAngle = Mathf.Infinity;
				for (int i = 0; i < hitCount; ++i){
					float angle;
					GameObject obj;
					if ((obj = WithinSight(transform, positionOffset, fieldOfViewAngle, viewDistance,
						    overlapColliders[i].gameObject, targetOffset, out angle, ignoreLayerMask, drawDebugRay)) !=
					    null){
						if (angle < minAngle){
							minAngle = angle;
							objectFound = obj;
						}
					}
				}
			}
			return objectFound;
		}

		public static GameObject WithinSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle,
			float viewDistance, GameObject targetObject, Vector3 targetOffset,
			out float angle, int ignoreLayerMask, bool drawDebugRay){
			if (targetObject == null){
				angle = 0;
				return null;
			}

			var direction = targetObject.transform.TransformPoint(targetOffset) -
				transform.TransformPoint(positionOffset);
			angle = Vector3.Angle(direction, transform.forward);
			direction.y = 0;
			if (direction.magnitude < viewDistance && angle < fieldOfViewAngle * 0.5f){
				var hitTransform = LineOfSight(transform, positionOffset, targetObject, targetOffset,
					ignoreLayerMask, drawDebugRay);
				if (hitTransform != null){
					if (IsAncestor(targetObject.transform, hitTransform)){
#if UNITY_EDITOR
						if (drawDebugRay){
							Debug.DrawLine(transform.TransformPoint(positionOffset),
								targetObject.transform.TransformPoint(targetOffset), Color.green);
						}
#endif
						return targetObject;
#if UNITY_EDITOR
					} else{
						if (drawDebugRay){
							Debug.DrawLine(transform.TransformPoint(positionOffset),
								targetObject.transform.TransformPoint(targetOffset), Color.yellow);
						}
#endif
					}
				} else if (!targetObject.TryGetComponent<Collider>(out _)){
					if (targetObject.gameObject.activeSelf){
						return targetObject;
					}
				}
			} else{
#if UNITY_EDITOR
				if (drawDebugRay){
					Debug.DrawLine(transform.TransformPoint(positionOffset),
						targetObject.transform.TransformPoint(targetOffset),
						angle >= fieldOfViewAngle * 0.5f ? Color.red : Color.magenta);
				}
#endif
			}
			return null;
		}

		public static bool IsAncestor(Transform target, Transform hitTransform){
			return hitTransform.IsChildOf(target) || target.IsChildOf(hitTransform);
		}

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
