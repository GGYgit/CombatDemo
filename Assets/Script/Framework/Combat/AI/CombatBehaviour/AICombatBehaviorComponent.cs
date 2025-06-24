using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// AI 战斗行为组件
	/// </summary>
	[Serializable]
	public class AICombatBehaviorComponent{
#region Config
		public AICombatStateType defaultCombatStateType = AICombatStateType.MeleeCombat;
		[LabelText("战斗状态")]
		public AICombatStateConfig[] aiCombatStateConfigs ={
			new(AICombatStateType.MeleeCombat),
			new(AICombatStateType.RangedCombat),
			new(AICombatStateType.ChaseTarget),
			new(AICombatStateType.StudyTarget),
			new(AICombatStateType.Flee),
		};
#endregion


		private EnemyController ownerController;

		/// <summary>
		/// 开始战斗
		/// </summary>
		public void StartCombat(EnemyController aiController){
			ownerController = aiController;
			//启用条件
			for (int i = 0; i < aiCombatStateConfigs.Length; i++){
				aiCombatStateConfigs[i].ConditionSet.Start(aiController);
			}
		}

		/// <summary>
		/// 退出战斗
		/// </summary>
		public void ExitCombat(){
			//停止条件
			for (int i = 0; i < aiCombatStateConfigs.Length; i++){
				aiCombatStateConfigs[i].ConditionSet.Stop();
			}
		}


		/// <summary>
		/// 根据目标距离取最佳战斗状态
		/// </summary>
		public AICombatStateType GetBestCombatStateByTargetDistance(){
			for (int i = 0; i < aiCombatStateConfigs.Length; i++){
				var combatState = aiCombatStateConfigs[i].aiCombatStateType;
				var combatConfig = aiCombatStateConfigs[i];
				if (EvaluateCombatState(combatState)){
					return combatConfig.aiCombatStateType;
				}
				combatConfig.ConditionSet.OnActionTriggered(ownerController);
			}
			return defaultCombatStateType;
		}


		/// <summary>
		/// 基于战斗圈分配圈匹配战斗状态
		/// </summary>
		public AICombatStateType GetBestCombatState(){
			for (int i = 0; i < aiCombatStateConfigs.Length; i++){
				var combatState = aiCombatStateConfigs[i].aiCombatStateType;
				var combatConfig = aiCombatStateConfigs[i];
				if (EvaluateCombatState(combatState)){ //满足状态配置的条件
					combatConfig.ConditionSet.OnActionTriggered(ownerController);
					return combatState;
				}
			}
			return defaultCombatStateType;
		}

		/// <summary>
		/// 获取可执行动作
		///
		/// </summary>
		/// <param name="actionChances"></param>
		/// <returns></returns>
		public EnemyAttackActionData TryGetExecuteAction(ActionChances actionChances){
			for (int i = 0; i < actionChances.enemyAttackActionData.Count; i++){
				if (CanExecuteAction(actionChances.enemyAttackActionData[i])){
					return actionChances.enemyAttackActionData[i];
				}
			}
			return null;
		}

		/// <summary>
		/// 1.随机概率
		/// 2.是否存在目标，目标是否拥有足够攻击Token
		/// 3.动作系统是否可以执行指定动作
		/// </summary>
		public bool CanExecuteAction(EnemyAttackActionData actionData){
			if (!ShouldExecuteAction(actionData)){
				return false;
			}
			var actionComp = ownerController.EnemyChar.ActionComp;
			if (ownerController.GetTarget() == null){
				return false;
			}
			if (ownerController.GetTargetFightCircle()){
				var fightingCircle = ownerController.GetTargetFightCircle();
				if (!fightingCircle.CanRegisterAction(this.ownerController, actionData.attackToken)){
					// Debug.Log("目标战斗圈Attack Token不足");
					return false;
				}
			} else{
				// Debug.Log("无目标战斗圈");
			}
			return actionComp.CanExecuteAction(actionData.actionTag, ActionPriority.Low);
		}

		/// <summary>
		/// 条件验证
		/// </summary>
		/// <param name="conditionSet"></param>
		/// <param name="aiController"></param>
		/// <param name="triggerConditionMetEvent"></param>
		/// <returns></returns>
		private bool VerifyCondition(AIActionConditionSet conditionSet, EnemyController aiController,
			bool triggerConditionMetEvent = true){
			if (conditionSet.IsConditionMet(aiController)){
				if (triggerConditionMetEvent)
					conditionSet.OnConditionSetIsMet(aiController);
				return true;
			}
			return false;
		}


		/// <summary>
		/// 几率判断，是否应该执行该动作
		/// </summary>
		private bool ShouldExecuteAction(EnemyAttackActionData actionData){
			return Random.Range(0, 100) < actionData.chancePercentage;
		}

		private bool EvaluateCombatState(AICombatStateType combatStateType){
			AICombatStateConfig combatStateConfig = GetCombatStateConfig(combatStateType);
			if (combatStateConfig != null){
				if (VerifyCondition(combatStateConfig.ConditionSet, ownerController)){
					return Random.Range(0, 100) <= combatStateConfig.TriggerChancePercentage;
				}
			}
			return false;
		}

		private AICombatStateConfig GetCombatStateConfig(AICombatStateType combatStateType){
			for (int i = 0; i < aiCombatStateConfigs.Length; i++){
				if (combatStateType == aiCombatStateConfigs[i].aiCombatStateType){
					return aiCombatStateConfigs[i];
				}
			}
			return null;
		}
	}
}
