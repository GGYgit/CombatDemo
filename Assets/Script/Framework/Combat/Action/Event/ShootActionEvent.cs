using UnityEngine;

namespace Framework.Combat.Runtime{
	public class ShootActionEvent : BaseActionEvent{
		public Projectile projectile;
		public float speedFactor=1;


		public override void OnEventStarted(BaseAction performingAction){
			base.OnEventStarted(performingAction);
			ShootComponent shootComp = performingAction.OwnerChar.GetCharComponent<ShootComponent>();
			if (shootComp == null){
				Debug.LogError("缺少射击组件，无法完成射击", performingAction.OwnerChar.gameObject);
				return;
			}
			TargetManagerComponent targetComp = performingAction.OwnerChar.GetCharComponent<TargetManagerComponent>();
			Vector3 dir = performingAction.OwnerChar.GetForwardVector();
			if (targetComp.HasTarget()){
				dir = targetComp.GetCurrentTarget().GetPosition() - performingAction.OwnerChar.GetPosition();
				dir.Normalize();
			}
			shootComp.Fire(projectile, dir,speedFactor);
		}
	}
}
