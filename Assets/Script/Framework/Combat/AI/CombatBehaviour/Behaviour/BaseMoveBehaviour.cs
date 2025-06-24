using UnityEngine;

namespace Framework.Combat.Runtime{
	public abstract class BaseMoveBehaviour : AIBehaviour{
		public MovementStateType movementStateType;
		public bool updateTargetPerFrame = true; //每帧更新导航目标位置
		public bool limitDurationTime = false; //限制追逐持续时间
		public Vector2 randomDuration = new Vector2(2f, 5f); //随机持续时间范围

		public bool lookAtTarget = true;
		public float rotateSpeed = 480f;//转向速度，s/°
		public float arriveDistance = 0.5f;

		private const float RotationEpsilon = 0.5f;
		private float arriveDist;
		private float duration;
		private BaseCharacter targetChar;

		public override void Start(EnemyController enemyController){
			base.Start(enemyController);
			duration = Random.Range(randomDuration.x, randomDuration.y);
			targetChar = controller.GetTarget();
			arriveDist = enemyController.GetTarget().GetEntityExtentRadius() + enemyChar.GetEntityExtentRadius() +
				arriveDistance;
			SetDestination();
		}

		public override BehaviourState Tick(EnemyController enemyController, float deltaTime){
			if (!enemyChar.MovementComp.CanMove()) return BehaviourState.Failure;
			if (HasArrived()){
				return BehaviourState.Success;
			}
			if (updateTargetPerFrame){
				SetDestination();
			}
			if (limitDurationTime){
				duration -= deltaTime;
				if (duration <= 0){
					return BehaviourState.Failure;
				}
			}
			if (lookAtTarget){
				enemyChar.MovementComp.RotateToPoint(targetChar.GetPosition(), rotateSpeed * deltaTime);
			}
			return BehaviourState.Running;
		}

		public override void End(EnemyController enemyController){
			controller.Movement.Stop();
		}


		/// <summary>
		/// 设置寻路目标
		/// </summary>
		protected abstract void SetDestination();

		/// <summary>
		/// 是否到达目的地
		/// </summary>
		protected bool HasArrived(){
			return enemyChar.navComp.HasReachedDestination(arriveDist);
		}
	}
}
