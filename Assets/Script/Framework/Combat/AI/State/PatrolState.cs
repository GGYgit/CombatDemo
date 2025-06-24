using UnityEngine;

namespace Framework.Combat.Runtime{
	public class PatrolState : BaseEnemyState{
		private Vector3 patrolTarget;
		private float waitTime = 2f;

		private float reachedTime;

		protected internal override void OnEnter(IFsm<EnemyController> fsm){
			base.OnEnter(fsm);
			SetRandomPatrolPoint();
			reachedTime = 0;
		}

		protected internal override void OnUpdate(IFsm<EnemyController> fsm, float deltaTime){
			base.OnUpdate(fsm, deltaTime);
			if (controller.Movement.HasReachedDestination()){
				reachedTime += deltaTime;//到达目标点后等待一段时间
				if (reachedTime > waitTime){
					SetRandomPatrolPoint();
				}
			}
			if (controller.TargetComp.HasTarget()){
				ChangeState<CombatState>(fsm);
			}
		}

		protected internal override void OnLeave(IFsm<EnemyController> fsm, bool isShutdown){
			base.OnLeave(fsm, isShutdown);
			controller.Movement.Stop();
		}

		private void SetRandomPatrolPoint(){
			reachedTime = 0;
			patrolTarget = character.GetPosition() + Random.insideUnitSphere * 6f;
			patrolTarget.y = character.GetPosition().y;
			controller.Movement.MoveTo(patrolTarget);
		}

	}
}
