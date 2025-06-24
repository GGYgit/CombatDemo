using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 视觉
	/// 先按照viewDistance与搜索层做球形物理检测，再通过忽略层的物理检测做视角及其他过滤
	/// </summary>
	public class SightAIPerception : AbstractAIPerception{
		[LabelText("视线角度")]
		public float fieldOfViewAngle = 90;
		[LabelText("视线距离")]
		public float viewDistance = 50;


		[LabelText("指定检测目标"), Tooltip("如果不为空，则检查列表内的对象"), FoldoutGroup("Advanced")]
		public List<GameObject> targetObjects;
		[LabelText("检测层Mask"), Tooltip("搜索检测目标的层级"), FoldoutGroup("Advanced")]
		public LayerMask objectLayerMask = 1 << 6;
		[LabelText("视线起点偏移值"), Tooltip("视线做遮挡检测时的射线终点偏移"), FoldoutGroup("Advanced")]
		public Vector3 offset = Vector3.up;
		[LabelText("视线目标偏移值"), FoldoutGroup("Advanced")]
		public Vector3 targetOffset = Vector3.up;
		[LabelText("检测时忽略自身"), FoldoutGroup("Advanced")]
		public bool disableAgentColliderLayer = true;
		[FoldoutGroup("Debug")]
		public bool drawDebugRay;
		[ReadOnly] [FoldoutGroup("Debug")]
		public GameObject returnedObject;

		// [LabelText("忽略层Mask"),Tooltip("视线检测时忽略的层")]
		private LayerMask ignoreLayerMask;
		private GameObject[] agentColliderGameObjects;
		private int[] originalColliderLayer;
		private Collider[] overlapColliders;
		private Collider2D[] overlap2DColliders;

		private const int maxCollisionCount = 200;
		private int ignoreRaycastLayer;

		private GameObject gameObject;
		private Transform transform;

		public override void Start(AIPerceptionComponent perceptionComp){
			base.Start(perceptionComp);
			ignoreRaycastLayer = LayerManager.IngoreIndex;
			ignoreLayerMask = LayerManager.EnemySightDetectionIgnore;
			gameObject = perceptionComp.enemyController.gameObject;
			transform = gameObject.transform;
			var colliders = gameObject.GetComponentsInChildren<Collider>();
			agentColliderGameObjects = new GameObject[colliders.Length];
			for (int i = 0; i < agentColliderGameObjects.Length; ++i){
				agentColliderGameObjects[i] = colliders[i].gameObject;
			}
			originalColliderLayer = new int[agentColliderGameObjects.Length];
			for (int i = 0; i < agentColliderGameObjects.Length; ++i){
				originalColliderLayer[i] = agentColliderGameObjects[i].layer;
			}
		}

		public override void Stop(){
		}

		public override void Tick(float deltaTime){
			Check();
		}

		protected void Check(){
			returnedObject = null;
			if (disableAgentColliderLayer){
				// Change the layer. Remember the previous layer so it can be reset after the check has completed.
				for (int i = 0; i < agentColliderGameObjects.Length; ++i){
					agentColliderGameObjects[i].layer = ignoreRaycastLayer;
				}
			}
			if (targetObjects != null && targetObjects.Count > 0){
				// If there are objects in the group list then search for the object within that list
				GameObject objectFound = null;
				float minAngle = Mathf.Infinity;
				for (int i = 0; i < targetObjects.Count; ++i){
					float angle;
					GameObject obj = PhysicsUtility.WithinSight(transform, offset, fieldOfViewAngle, viewDistance,
						targetObjects[i], targetOffset, out angle, ignoreLayerMask,
						drawDebugRay);
					if (obj != null){
						// This object is within sight. Set it to the objectFound GameObject if the angle is less than any of the other objects
						if (angle < minAngle){
							minAngle = angle;
							objectFound = obj;
						}
					}
				}
				returnedObject = objectFound;
			} else{
				// If the target object is null and there is no tag then determine if there are any objects within sight based on the layer mask
				if (overlapColliders == null || overlapColliders.Length == 0){
					overlapColliders = new Collider[maxCollisionCount];
				}
				returnedObject = PhysicsUtility.WithinSight(transform, offset, fieldOfViewAngle, viewDistance,
					overlapColliders, objectLayerMask, targetOffset, ignoreLayerMask,
					drawDebugRay);
			}
			if (disableAgentColliderLayer){
				for (int i = 0; i < agentColliderGameObjects.Length; ++i){
					agentColliderGameObjects[i].layer = originalColliderLayer[i];
				}
			}
			if (returnedObject != null){
				OnPerceive(returnedObject);
			}
			// // An object is not within sight so return failure
			// return TaskStatus.Failure;
		}

		public override void OnGizmos(GameObject gameObject){
			// PhysicsUtility.DrawLineOfSight(gameObject.transform, offset, fieldOfViewAngle, 0, viewDistance, false);
		}
	}
}
