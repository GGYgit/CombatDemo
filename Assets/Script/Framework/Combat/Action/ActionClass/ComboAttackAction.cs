using System;
using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 连击动作类，有输入触发的连击与传入autoPlay自动触发所有连击
	/// </summary>
	[Serializable]
	public class ComboAttackAction : BaseAction{
		public const float inputCacheTime = 0.5f;
		public AnimationConfig[] animationList = new AnimationConfig[0];

		private WeaponEntity weaponEntity;
		private AnimationConfig currentAnimationConfig;
		private WeaponComponent weaponComponent;

		private int comboIndex = 0;
		private int maxComboIndex = 0;
		private float triggerCombo = 0;
		private bool autoPlay = false;

		public void ActiveDamage(DamageConfig damageConfig){
			weaponComponent?.ActiveDamage(damageConfig);
		}

		public void DeActiveDamage(){
			weaponComponent?.DeActiveDamage();
		}

		protected override void OnActivated(object userdata = null){
			if (userdata != null){
				autoPlay = (bool) userdata;
			}
			weaponComponent = actionComp.Owner.GetCharComponent<WeaponComponent>();
			triggerCombo = 0;
			maxComboIndex = animationList.Length;
			comboIndex = 0;
			Play(animationList[comboIndex]);
		}

		protected override void OnReceiveInput(string newActionTag, ActionPriority actionPriority,
			object userdata = null){
			if (actionTag == newActionTag){
				triggerCombo = inputCacheTime;
			}
		}

		private void Play(AnimationConfig animationConfig){
			this.currentAnimationConfig = animationConfig;
			ExecuteAction(currentAnimationConfig);
		}


		protected override void OnTick(float deltaTime){
			triggerCombo -= deltaTime;
			if (ActionPhaseFlag.HasFlag(ActionPhaseFlagType.Continue) && (triggerCombo > 0 || autoPlay) &&
			    comboIndex < maxComboIndex - 1){
				ActionPhaseFlag &= ~ActionPhaseFlagType.Continue;
				triggerCombo = 0;
				comboIndex++;
				Play(animationList[comboIndex]);
			}
		}
	}
}
