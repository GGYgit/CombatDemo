using System.Collections.Generic;
using UnityEngine;

namespace Framework.Combat.Runtime{
	public sealed class DamageComponent : CharacterComponent{
		private Dictionary<int, List<GameObject>> hitColliders = new Dictionary<int, List<GameObject>>();

		/// <summary>
		/// 激活伤害检测
		/// </summary>
		public void ActivateDamage(HitBox[] hitBoxes, DamageConfig damageConfig){
			ClearHitTarget();
			for (int i = 0; i < hitBoxes.Length; i++){
				hitBoxes[i].Active(this);
				hitBoxes[i].SetDamageConfig(damageConfig);
			}
		}


		public void DeactivateDamage(HitBox[] hitBoxes){
			for (int i = 0; i < hitBoxes.Length; i++){
				hitBoxes[i].DeActive(this);
			}
		}


		public void HitGameObject(HitInfo hitInfo, Collider hitCol){
			//1. GO检测
			if (!CheckHitGameObject(hitInfo, hitCol, out GameObject hitTargetObj)) return;
			//2. Character检测
			if (!CheckHitCharacter(hitTargetObj, out var hitCharacter)) return;
			OnHit(hitInfo, hitCharacter);
		}

		/// <summary>
		/// 检测命中的GameObject是否有效
		/// 当前可以造成伤害，未被记录过命中信息，不等于当前自身角色
		/// </summary>
		private bool CheckHitGameObject(HitInfo hitInfo, Collider other, out GameObject hitTargetObj){
			hitTargetObj = null;
			var targetObj = GetHitEntityGameObject(other);
			if (!hitColliders.ContainsKey(hitInfo.hitId))
				hitColliders.Add(hitInfo.hitId, new List<GameObject>());
			if (!hitColliders[hitInfo.hitId].Contains(targetObj) && targetObj != owner.gameObject){
				hitTargetObj = targetObj;
				return true;
			}
			return false;
		}

		/// <summary>
		/// 检测命中的角色是否有效
		/// </summary>
		/// <param name="hitTarget"></param>
		/// <returns></returns>
		private bool CheckHitCharacter(GameObject hitTarget, out ICombatEntity hitCharacter){
			hitCharacter = hitTarget.GetComponent<ICombatEntity>();
			if (hitCharacter == null){
				return false;
			}
			if (!owner.GetEntityCombatTeam().IsEnemyTeam(hitCharacter.GetEntityCombatTeam())) return false; //判断是否敌对阵营
			return true;
		}


		/// <summary>
		/// 获取受击的实体对象
		/// </summary>
		private GameObject GetHitEntityGameObject(Collider collider){
			ICombatEntity collisionReceiverObject = collider.GetComponent<ICombatEntity>();
			if (collisionReceiverObject != null && collisionReceiverObject.gameObject != null){
				return collisionReceiverObject.gameObject;
			}
			var targetObj = collider.gameObject;
			Rigidbody hitRigidbody = collider.attachedRigidbody;
			if (hitRigidbody != null){
				targetObj = hitRigidbody.gameObject;
			}
			return targetObj;
		}

		private void OnHit(HitInfo hitInfo, ICombatEntity hitCharacter){
			if (hitCharacter == null){
				return;
			}
			DamageInfo damageInfo = DamageInfo.Create(hitInfo.damageConfig);
			damageInfo.damageCauser = owner;
			if (!hitCharacter.CheckCanTakeDamage()){
				return;
			}
			if (hitInfo.hitId >= 0){
				hitColliders[hitInfo.hitId].Add(hitCharacter.gameObject);
			}
			damageInfo.damageReceiver = hitCharacter;
			damageInfo.hitPosition = hitInfo.hitPosition;
			var afterDamageInfo = ApplyDamage(hitCharacter, damageInfo, hitInfo);
			GameEntry.Time.FreezeFrame(afterDamageInfo.freezeFrame);
			if (afterDamageInfo.cameraShakeLevel > 0){
				GameEntry.Main.cameraController.ShakeCamera(afterDamageInfo.cameraShakeLevel);
			}
		}

		private DamageInfo ApplyDamage(ICombatEntity character, DamageInfo damageInfo, HitInfo hitInfo){
			damageInfo.damageValue = owner.AttributeComp.GetCurrentValue(AttributeConstant.AttackPower) *
				hitInfo.damageConfig.damageFactor;
			owner.DealDamage(character, damageInfo);
			character.TakeDamage(damageInfo);
			return damageInfo;
		}


		/// <summary>
		///清理已记录的碰撞目标
		/// </summary>
		private void ClearHitTarget(){
			foreach (var targetCollider in hitColliders){
				targetCollider.Value.Clear();
			}
		}
	}
}
