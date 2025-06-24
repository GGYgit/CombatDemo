namespace Framework.Combat.Runtime{
	public class MoveToTargetBehaviour : BaseMoveBehaviour{
		/// <summary>
		/// 移动至目标所在位置
		/// </summary>
		protected override void SetDestination(){
			BaseCharacter targetChar = controller.GetTarget();
			controller.Movement.SetMovementState(movementStateType);
			controller.Movement.MoveTo(targetChar.GetPosition());
		}
	}
}
