using System;
using UnityEngine;

namespace Framework.Combat.Runtime{
	[Serializable]
	public class AIAttackBehaviour : AIBehaviour{
		private const float threshold = 0.95f;
		public ActionChances actionChances;

		private AICombatBehaviorComponent combatBehaviorComp;
		private ActionState actionState;
		private EnemyAttackActionData attackActionData;
		private BaseCharacter targetChar;
		private float rotateDuration;

		public override void Start(EnemyController enemyController){
			base.Start(enemyController);
			targetChar = enemyController.GetTarget();
			combatBehaviorComp = enemyController.combatBehaviorComp;
			actionState = ActionState.Wait;
			attackActionData = combatBehaviorComp.TryGetExecuteAction(actionChances);
			if (attackActionData == null){
				actionState = ActionState.Fail;
			} else actionState = ActionState.Wait;
		}


		public override BehaviourState Tick(EnemyController enemyController, float deltaTime){
			//wait
			switch (actionState){
				case ActionState.Wait:
					//等待朝向，完成朝向后再触发动作
					if (attackActionData.waitLockAtTarget){
						rotateDuration += deltaTime;
						Vector3 toTarget = (targetChar.GetPosition() - enemyChar.GetPosition()).normalized;
						Vector3 forward = targetChar.GetForwardVector();
						toTarget.y = forward.y = 0;
						float dot = Vector3.Dot(forward, toTarget);
						if (dot > threshold || rotateDuration >= attackActionData.maxLookAtRotationMaxTime){
							TriggerAction();
						} else{
							enemyChar.MovementComp.RotateToDirection(toTarget, attackActionData.lookAtRotationSpeed);
						}
					} else{
						TriggerAction();
					}
					return BehaviourState.Running;
				case ActionState.Run:
					return BehaviourState.Running;
				case ActionState.End:
					return BehaviourState.Success;
				case ActionState.Fail:
					return BehaviourState.Failure;
				default:
					throw new ArgumentOutOfRangeException();
			}
			return BehaviourState.Running;
		}

		public override void End(EnemyController enemyController){
			enemyChar.ActionComp.OnActionEnd -= OnActionEnd;
		}

		private void OnActionEnd(string actionTag){
			actionState = ActionState.End;
		}

		private void TriggerAction(){
			bool isTriggered = enemyChar.ActionComp.TriggerAction(attackActionData.actionTag, ActionPriority.Low, true);
			if (isTriggered){
				actionState = ActionState.Run;
				enemyChar.ActionComp.OnActionEnd += OnActionEnd;
			} else actionState = ActionState.Fail;
		}

		private enum ActionState{
			Wait,
			Run,
			End,
			Fail, //无可执行动作
		}
	}
}
