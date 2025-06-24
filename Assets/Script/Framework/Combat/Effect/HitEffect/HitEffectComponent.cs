using System;
using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 受击效果组件
	/// </summary>
	public class HitEffectComponent : CharacterComponent{
		[SerializeReference]
		public BaseHitEffect[] hitEffects = Array.Empty<BaseHitEffect>();
		[SerializeReference]
		public BaseHitEffect[] deadEffects = Array.Empty<BaseHitEffect>();

		protected override void OnInit(BaseCharacter character){
			base.OnInit(character);
			foreach (var hitEffect in hitEffects){
				hitEffect.Init(this);
			}
			foreach (var deadEffect in deadEffects){
				deadEffect.Init(this);
			}
		}

		void Update(){
			if (owner == null) return;
			float deltaTime = owner.DeltaTime;
			foreach (var hitEffect in hitEffects){
				hitEffect.Tick(deltaTime);
			}
			foreach (var deadEffect in deadEffects){
				deadEffect.Tick(deltaTime);
			}
		}


		public void TriggerHitEffect(DamageInfo damageInfo){
			foreach (var hitEffect in hitEffects){
				hitEffect.Trigger(damageInfo);
			}
		}


		public void TriggerDeadEffect(DamageInfo damageInfo){
			foreach (var deadEffect in deadEffects){
				deadEffect.Trigger(damageInfo);
			}
		}

		private void OnDestroy(){
			foreach (var hitEffect in hitEffects){
				hitEffect.Shutdown();
			}
		}
	}
}
