using UnityEngine;

namespace Framework.Combat.Runtime{
	public class PlayerCharacter : BaseCharacter{
		private WeaponComponent weaponComponent;
		private CharacterController cc;

		/// <summary>
		/// 造成伤害时触发一次相机软锁定修正
		/// </summary>
		protected override void OnDealDamage(ICombatEntity targetEntity, DamageInfo damageInfo){
			base.OnDealDamage(targetEntity, damageInfo);
		}

		public override void DisableCollider(){
			cc.enabled = false;
		}

		public override void EnableCollider(){
			cc.enabled = true;
		}


		public override void PossessedBy(BaseController newController){
			base.PossessedBy(newController);
			cc = GetComponent<CharacterController>();
		}


		public void SetupPlayerInputComponent(PlayerInputAction.GamePlayActions action){
			var playerMovement = MovementComp as PlayerMovementComponent;
			Debug.Assert(playerMovement);
		}
	}
}
