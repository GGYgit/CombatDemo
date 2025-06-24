using UnityEngine;

namespace Framework.Combat.Runtime{
	public class EffectFollowTarget : MonoBehaviour{
		public Transform target;
		public Vector3 offset = Vector3.zero;
		public Quaternion rotationOffset = Quaternion.identity;

		void Update(){
			if (target != null){
				transform.position = target.position + offset;
				transform.rotation = target.rotation * rotationOffset;
			}
		}
	}
}
