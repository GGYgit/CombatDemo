namespace Framework.Combat.Runtime{
	/// <summary>
	/// 针对武器连击动作类的伤害检测事件
	/// </summary>
	public class WeaponDamageEvent : BaseActionEvent{
		public DamageConfig damageConfig;

		public override void OnEventStarted(BaseAction performingAction){
			base.OnEventStarted(performingAction);
			var weaponComp = performingAction.OwnerChar.GetCharComponent<WeaponComponent>();
			if (weaponComp != null){
				weaponComp.ActiveDamage(damageConfig);
			}
		}

		public override void OnEventEnded(BaseAction performingAction){
			base.OnEventEnded(performingAction);
			var weaponComp = performingAction.OwnerChar.GetCharComponent<WeaponComponent>();
			if (weaponComp != null){
				weaponComp.DeActiveDamage();
			}
		}

		public override void OnActionEnded(BaseAction performingAction){
			base.OnActionEnded(performingAction);
			var weaponComp = performingAction.OwnerChar.GetCharComponent<WeaponComponent>();
			if (weaponComp != null){
				weaponComp.DeActiveDamage();
			}
		}
	}
}
