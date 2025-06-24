using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 战斗状态，读取配置跑行为序列
	/// </summary>
	public class CombatState : BaseEnemyState{
		private AICombatBehaviorComponent combatBehaviour;
		private AICombatStateType combatStateType;
		private AIBehaviourSequence currentSequence;
		private CircleType circleType;

		public CircleType CircleType => circleType;
		public AICombatStateType CombatStateType => combatStateType;
		public AIBehaviourSequence CurrentSequence => currentSequence;


		protected internal override void OnInit(IFsm<EnemyController> fsm){
			base.OnInit(fsm);
			combatBehaviour = fsm.Owner.combatBehaviorComp;
		}

		protected internal override void OnDestroy(IFsm<EnemyController> fsm){
			base.OnDestroy(fsm);
		}

		protected internal override void OnEnter(IFsm<EnemyController> fsm){
			base.OnEnter(fsm);
			combatBehaviour.StartCombat(controller);
			ReassignCombatBehaviour();
		}

		protected internal override void OnUpdate(IFsm<EnemyController> fsm, float deltaTime){
			base.OnUpdate(fsm, deltaTime);
			currentSequence.Update(controller, deltaTime);
			if (currentSequence.IsFinished){
				if (!controller.TargetComp.HasTarget())
					ChangeState<IdleState>(fsm);
				else{ //重新分配行为
					ReassignCombatBehaviour();
				}
			}
		}

		protected internal override void OnLeave(IFsm<EnemyController> fsm, bool isShutdown){
			base.OnLeave(fsm, isShutdown);
			combatBehaviour.ExitCombat();
			currentSequence?.Interrupt();
		}

		/// <summary>
		/// 决策战斗状态，执行行为队列
		/// </summary>
		private void ReassignCombatBehaviour(){
			BaseCharacter targetChar = controller.GetTarget();
			FightingCircle fightingCircle = targetChar.GetCharComponent<FightingCircle>();
			//向战斗圈注册
			fightingCircle?.Register(controller, out circleType);
			//取战斗状态
			combatStateType = combatBehaviour.GetBestCombatState();
			//根据战斗状态取行为队列
			AICombatStateConfig combatStateConfig = null;
			foreach (var stateConfig in combatBehaviour.aiCombatStateConfigs){
				if (stateConfig.aiCombatStateType != combatStateType) continue;
				combatStateConfig = stateConfig;
				break;
			}
			if (combatStateConfig == null){
				Debug.Log("缺少战斗状态的配置");
				return;
			}
			currentSequence = combatStateConfig.behaviourSequence;
			currentSequence.Start(controller);
			controller.CombatStateType = combatStateType;
		}
	}
}
