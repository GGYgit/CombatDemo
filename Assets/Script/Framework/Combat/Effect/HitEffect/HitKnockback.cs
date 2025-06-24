using System;
using UnityEngine;

namespace Framework.Combat.Runtime{
	[Serializable]
	public class HitKnockback : BaseHitEffect{
		public float drag = 5f;
		public bool IsKnockedBack{ get; private set; }
		private BaseCharacter character;
		private Vector3 knockbackVelocity;
		private float threshold = 0.1f;
		public override void Init(HitEffectComponent comp){
			character = comp.Owner;
		}

		public override void Shutdown(){
		}

		public override void Trigger(DamageInfo damageInfo){
			Vector3 direction = damageInfo.damageReceiver.transform.position -
				damageInfo.damageCauser.transform.position;
			knockbackVelocity = direction.normalized * damageInfo.knockBackDistance;
			IsKnockedBack = true;
		}

		public override void Tick(float deltaTime){
			if (IsKnockedBack){
				character.Move(knockbackVelocity * deltaTime);
				// 逐渐减速
				knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, drag * deltaTime);
				if (knockbackVelocity.magnitude < threshold){
					knockbackVelocity = Vector3.zero;
					IsKnockedBack = false;
				}
			}
		}
	}
}
