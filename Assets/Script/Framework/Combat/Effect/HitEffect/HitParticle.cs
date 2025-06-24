using System;
using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 受击粒子特效
	/// </summary>
	[Serializable]
	public class HitParticle:BaseHitEffect{
		public string[] particleNameList;
		private FisherYatesRandom random;
		public override void Init(HitEffectComponent comp){
			random = new FisherYatesRandom();
		}

		public override void Shutdown(){
		}

		public override void Trigger(DamageInfo damageInfo){
			string particleName = GetRandomParticleName();
			Transform damageReceiver = damageInfo.damageReceiver.transform;
			//特效朝向等于伤害点与受击方的反方向
			var damageDirection = damageInfo.hitPosition - new Vector3(damageReceiver.position.x, damageInfo.hitPosition.y, damageReceiver.position.z);

			// var damageDirection = damage.hitPosition - transform.position;
			var hitRotation = damageDirection != Vector3.zero
				? Quaternion.LookRotation(damageDirection)
				: damageReceiver.rotation;
			GameEntry.Effect.PlayOnceEffect(particleName, damageInfo.hitPosition, hitRotation);
		}

		public override void Tick(float deltaTime){
		}

		private string GetRandomParticleName(){
			int idx = random.Next(particleNameList.Length);
			return particleNameList[idx];
		}
	}
}
