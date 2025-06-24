namespace Framework.Combat.Runtime{
	/// <summary>
	/// 监听该动作是否命中敌人，如命中会进行软修正摄像机到玩家正面
	/// 离开该动作时会停止摄像机的软修正
	/// </summary>
	public class CameraSoftLookAtEvent : BaseActionEvent{
		public override void OnActionStarted(BaseAction performingAction){
			performingAction.OwnerChar.OnDealDamageEvent += OnDealDamageEvent;
		}

		protected void OnDealDamageEvent(DamageInfo damageInfo){
			GameEntry.Main.cameraController.BeginCameraSoftLookAt(length);
		}

		public override void OnActionEnded(BaseAction performingAction){
			performingAction.OwnerChar.OnDealDamageEvent -= OnDealDamageEvent;
			GameEntry.Main.cameraController.StopCameraSoftLookAt();
		}
	}
}
