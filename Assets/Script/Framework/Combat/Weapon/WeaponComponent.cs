using System.Collections.Generic;
using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 武器组件，控制武器实例化及武器攻伤害检测开关
	/// </summary>
	public class WeaponComponent : CharacterComponent{
		public WeaponEntity swordPrefab;
		public Transform spawnRoot;
		public int initialSwordCount = 1;
		private List<WeaponEntity> weapons = new List<WeaponEntity>();

		protected override void OnInit(BaseCharacter character){
			for (int i = 0; i < initialSwordCount; i++){
				AddWeapon();
			}
		}

		public void ActiveDamage(DamageConfig damageConfig){
			foreach (var weaponEntity in weapons){
				weaponEntity.ActiveDamage(damageConfig);
			}
		}

		public void DeActiveDamage(){
			foreach (var weaponEntity in weapons){
				weaponEntity.DeActiveDamage();
			}
		}


		public void AddWeapon(){
			if (swordPrefab == null) return;
			WeaponEntity newWeapon = Instantiate(swordPrefab, spawnRoot);
			newWeapon.Init(this);
			newWeapon.transform.localPosition = Vector3.zero;
			newWeapon.transform.localRotation = Quaternion.identity;
			weapons.Add(newWeapon);
			AttachWeapon(newWeapon);
		}

		public void RemoveWeapon(){
			if (weapons.Count == 0) return;
			WeaponEntity weaponToRemove = weapons[weapons.Count - 1];
			UnAttachWeapon(weaponToRemove);
			weapons.RemoveAt(weapons.Count - 1);
			Destroy(weaponToRemove.gameObject);
		}

		public void UnAttachWeapon(WeaponEntity weapon){
			weapon.transform.SetParent(null);
		}

		public void AttachWeapon(WeaponEntity newWeapon){
			if (owner == null){
				Destroy(newWeapon);
			}
			newWeapon.transform.SetParent(spawnRoot);
		}
	}
}
