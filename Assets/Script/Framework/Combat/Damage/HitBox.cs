using UnityEngine;

namespace Framework.Combat.Runtime{
	public class HitBox : MonoBehaviour{
		public int hitId;
		public string hitBoxName = string.Empty;
		public bool alwaysShowGizmos = false;

		private DamageConfig damageConfig;
		private DamageComponent damageComp;
		private Collider hitBoxCollider;

		private void Awake(){
			gameObject.layer = LayerManager.HitBoxIndex;
			hitBoxCollider = GetComponent<Collider>();
			hitBoxCollider.isTrigger = true;
			hitBoxCollider.enabled = false;
		}

		public void Active(DamageComponent damageComponent){
			this.damageComp = damageComponent;
			enabled = true;
			hitBoxCollider.enabled = true;
		}

		public void SetDamageConfig(DamageConfig damageConfig){
			this.damageConfig = damageConfig;
		}

		public void DeActive(DamageComponent damageComponent){
			this.damageComp = null;
			damageConfig = null;
			enabled = false;
			hitBoxCollider.enabled = false;
		}

		private void OnTriggerEnter(Collider other){
			if (!TriggerCondition(other)) return;
			Vector3 hitPos = other.ClosestPoint(transform.position);
			damageComp?.HitGameObject(new HitInfo(hitId, hitPos, damageConfig), other);
		}


		private bool TriggerCondition(Collider other){
			if (other.gameObject.layer != LayerManager.Trigger && !other.isTrigger){
				return true;
			}
			return false;
		}

		private void OnDrawGizmosSelected(){
			if (!alwaysShowGizmos)
				DrawGizmos();
		}

		private void OnDrawGizmos(){
			if (alwaysShowGizmos)
				DrawGizmos();
		}

		private void DrawGizmos(){
			Color color = Color.green;
			color.a = 0.6f;
			Gizmos.color = color;
			BoxCollider trigger = gameObject.GetComponent<BoxCollider>();
			if (!Application.isPlaying && trigger && !trigger.enabled) trigger.enabled = true;
			if (trigger && trigger.enabled){
				if (trigger as BoxCollider){
					BoxCollider box = trigger as BoxCollider;
					Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
					Gizmos.DrawCube(box.center, Vector3.Scale(Vector3.one, box.size));
				}
			}
		}
	}
}
