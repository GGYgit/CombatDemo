using UnityEngine;

namespace Framework.Combat.Runtime{
	public class IdleState : BaseEnemyState{

		protected internal override void OnEnter(IFsm<EnemyController> fsm){
			base.OnEnter(fsm);
		}

		protected internal override void OnLeave(IFsm<EnemyController> fsm, bool isShutdown){
			base.OnLeave(fsm, isShutdown);
		}

		protected internal override void OnUpdate(IFsm<EnemyController> fsm, float deltaTime){
			if (controller.TargetComp.HasTarget()){
				// Debug.Log("存在仇恨目标，进入战斗");
				ChangeState<CombatState>(fsm);
			}
		}

	}
}
