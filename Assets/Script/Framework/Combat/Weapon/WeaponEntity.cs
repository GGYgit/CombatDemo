using UnityEngine;
using XftWeapon;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 武器实例，包含近战攻击及远程攻击逻辑
	/// </summary>
	public class WeaponEntity : MonoBehaviour{
		private DamageComponent damageComp;
		private HitBox[] hitBoxes;
		private XWeaponTrail trail;
		public void Init(WeaponComponent weaponComponent){
			damageComp = weaponComponent.Owner.GetCharComponent<DamageComponent>();
			hitBoxes = GetComponentsInChildren<HitBox>();
			trail = GetComponentInChildren<XWeaponTrail>();
			trail?.Deactivate();
		}

		public void ActiveDamage(DamageConfig damageConfig){
			damageComp.ActivateDamage(hitBoxes, damageConfig);
			trail?.Activate();
		}

		public void DeActiveDamage(){
			damageComp.DeactivateDamage(hitBoxes);
			trail?.Deactivate();
		}
	}
}
