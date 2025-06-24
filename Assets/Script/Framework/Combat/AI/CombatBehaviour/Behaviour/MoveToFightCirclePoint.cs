using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 移动至当前FightCircle分配的目标点
	/// </summary>
	public class MoveToFightCirclePoint : BaseMoveBehaviour{
		private float random;

		public override void Start(EnemyController enemyController){
			base.Start(enemyController);
			random = Random.Range(0f, 1f);
		}

		protected override void SetDestination(){
			FightingCircle fightingCircle = controller.GetTargetFightCircle();
			Vector3 targetPoint = Vector3.zero;
			if (fightingCircle == null){
				Debug.LogError("无目标，或目标无FightingCircle");
				return;
			}
			if (fightingCircle.GetMeleeCircle().IsContains(controller)){
				targetPoint = fightingCircle.GetSlotPositionFromMeleeCircle(controller);
			} else if (fightingCircle.GetApproachCircle().IsContains(controller)){
				var points = fightingCircle.GetSlotPositionFromApproachCircle(controller);
				targetPoint = Vector3.Lerp(points[0], points[1], random); //随机取两点之间
			} else if (fightingCircle.GetRangeCircle().IsContains(controller)){
				targetPoint =
					fightingCircle.GetSlotPositionFromRangeCircle(controller, controller.GetTarget().GetPosition());
				// targetPoint = transform.position;
			} else{
				targetPoint = enemyChar.GetPosition();
			}
			controller.Movement.SetMovementState(movementStateType);
			enemyChar.navComp.MoveTo(targetPoint);
		}
	}
}
