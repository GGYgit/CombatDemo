using System;
using UnityEngine;

namespace Framework.Combat.Runtime{
	[Serializable]
	public class WaitBehaviour : AIBehaviour{
		public float waitTime = 0.25f;
		public bool lookAtTarget = true;
		public float rotateSpeed = 480f; //转向速度，s/°
		private float elapsed;

		public override void Start(EnemyController enemyController){
			base.Start(enemyController);
			elapsed = 0;
		}

		public override void End(EnemyController enemyController){
		}

		public override BehaviourState Tick(EnemyController enemy, float deltaTime){
			elapsed += deltaTime;
			if (elapsed >= waitTime){
				return BehaviourState.Success;
			}
			if (lookAtTarget){
				BaseCharacter targetChar = enemy.GetTarget();
				enemyChar.MovementComp.RotateToPoint(targetChar.GetPosition(), rotateSpeed * deltaTime);
			}
			return BehaviourState.Running;
		}
	}
}
