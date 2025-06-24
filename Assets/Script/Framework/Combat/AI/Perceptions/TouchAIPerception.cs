using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 触觉
	/// 先做距离的物理检测，之后再做可视的检测判断是否有墙壁阻挡
	/// </summary>
	public class TouchAIPerception : AbstractAIPerception{
		// public GameObject targetObject;
		// public string targetTag;
		[LabelText("范围"), OnValueChanged("OnMagnitudeChange")]
		public float magnitude = 5;

		[LabelText("搜索层Mask"), FoldoutGroup("Advanced")]
		public LayerMask objectLayerMask = 1 << 6;
		[LabelText("检测起点偏移"), FoldoutGroup("Advanced")]
		public Vector3 offset;
		[LabelText("视线阻挡检测"), FoldoutGroup("Advanced")]
		public bool lineOfSight;
		[LabelText("视线检测目标点偏移"), ShowIf("lineOfSight"), FoldoutGroup("Advanced")]
		public Vector3 targetOffset = Vector3.up;
		[FoldoutGroup("Debug")]
		public bool drawDebugRay;
		[FoldoutGroup("Debug")]
		public GameObject returnedObject;

		private List<GameObject> objects = new List<GameObject>();
		// distance * distance, optimization so we don't have to take the square root
		private float sqrMagnitude;
		private bool overlapCast = true;
		private LayerMask ingoreMask;
		private GameObject gameObject;
		private Transform transform;

#if UNITY_EDITOR
		private void OnMagnitudeChange(){
			sqrMagnitude = magnitude * magnitude;
		}
#endif
		public override void Start(AIPerceptionComponent perceptionComp){
			base.Start(perceptionComp);
			sqrMagnitude = magnitude * magnitude;
			gameObject = perceptionComp.enemyController.gameObject;
			transform = gameObject.transform;
			ingoreMask = LayerManager.EnemySightDetectionIgnore;
		}

		public override void Stop(){
		}

		public override void Tick(float deltaTime){
			Check();
		}

		private static Collider[] results = new Collider[200];

		public void Check(){
			returnedObject = null;
			if (transform == null /*|| objects == null*/)
				return;
			if (overlapCast){
				objects.Clear();
				var size = Physics.OverlapSphereNonAlloc(transform.position, magnitude, results, objectLayerMask);
				for (int i = 0; i < size; ++i){
					objects.Add(results[i].gameObject);
				}
			}
			Vector3 direction;
			for (int i = 0; i < objects.Count; ++i){
				if (objects[i] == null || objects[i] == gameObject){
					continue;
				}
				direction = objects[i].transform.position - (transform.position + offset);
				if (Vector3.SqrMagnitude(direction) < sqrMagnitude){
					if (lineOfSight){
						var hitTransform = PhysicsUtility.LineOfSight(transform, offset, objects[i], targetOffset, ingoreMask, drawDebugRay);
						if (hitTransform != null){
							returnedObject = objects[i];
						}
					} else{
						returnedObject = objects[i];
					}
				}
			}
			if (returnedObject != null)
				OnPerceive(returnedObject);
			// else OnLostTarget();
			return;
		}

		public override void OnGizmos(GameObject gameObject){
#if UNITY_EDITOR
			if (magnitude <= 0){
				return;
			}
			var oldColor = UnityEditor.Handles.color;
			UnityEditor.Handles.color = Color.yellow;
			UnityEditor.Handles.DrawWireDisc(gameObject.transform.position + offset, gameObject.transform.up,
				magnitude);
			UnityEditor.Handles.color = oldColor;
#endif
		}
	}
}
