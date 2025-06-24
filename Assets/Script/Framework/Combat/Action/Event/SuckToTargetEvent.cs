using UnityEngine;
using UnityEngine.UI;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 吸附至目标事件，距离内存在目标时自动将吸附到目标处。如无目标将转向输入方向
	/// </summary>
	public class SuckToTargetEvent : BaseActionEvent{
		public float speed = 30f; //移动速度
		public float stopDistance = 3f; //最佳攻击距离
		public float suckRange = 4f;
		// public bool cameraAdjust = true;
		public float rotationSpeed = 2340;

		private BaseCharacter owner;
		private bool hasTarget;
		private Vector3 targetPosition;
		private float targetRadius;

		public override void OnEventStarted(BaseAction performingAction){
			owner = performingAction.OwnerChar;
			var targetComp = owner.GetCharComponent<TargetManagerComponent>();
			if (targetComp){
				BaseCharacter target = targetComp.GetCurrentTarget();
				if (target == null){
					target = targetComp.GetNearestTarget(suckRange);
				}
				hasTarget = target != null;
				if (hasTarget){
					targetRadius = target.GetEntityExtentRadius();
					targetPosition = target.GetPosition();
				}
			} else hasTarget = false;
		}

		public override void OnEventEnded(BaseAction performingAction){
		}

		public override void OnTick(float deltaTime){
			var currentSpeed = speed;
			if (hasTarget){
				var ownerPosition = owner.GetPosition();
				Vector3 vector = targetPosition - ownerPosition;
				targetPosition.y = vector.y = 0;
				float distance = owner.GetPlanarDistanceTo(targetPosition);
				distance -= owner.GetEntityExtentRadius() + targetRadius;
				owner.MovementComp.RotateToPoint(targetPosition, rotationSpeed * deltaTime);
				if (distance <= stopDistance) return;
				owner.MovementComp.Move(vector.normalized * (currentSpeed * deltaTime));
			} else{
				Vector3 direction = owner.MovementComp.GetMoveInput().GetCameraRelativeDirection();
				owner.MovementComp.RotateToDirection(direction, rotationSpeed * deltaTime);
			}
		}
	}
}
