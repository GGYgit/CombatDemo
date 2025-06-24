using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// AI状态
	/// </summary>
	public enum AIState{
		Patrol = 0,
		Combat = 1,
		ReturnHome = 2,
		Wait = 3,
	}

	public enum MovementStateType{
		Jog,
		Sprint,
	}

	/// <summary>
	/// 战斗圈类型
	/// </summary>
	public enum CircleType : byte{
		Melee,
		Approach,
		Range,
		Wait,
	}

	public enum AICombatStateType : byte{
		[LabelText("近战")] MeleeCombat = 0,
		[LabelText("追击")] ChaseTarget = 1,
		[LabelText("远程")] RangedCombat = 2,
		[LabelText("观察目标")] StudyTarget = 3,
		[LabelText("远离目标")] Flee = 4,
	};

	public enum CombatStyle : byte{
		[LabelText("近战")] Melee,
		[LabelText("远程")] Range,
		[LabelText("近战与远程")] Both,
	}


	[Serializable]
	public class EnemyAttackActionData{
		[VerticalGroup("Split")]
		public string actionTag;
		[LabelText("攻击Token消耗"), FoldoutGroup("Split/$actionTag", false)]
		public float attackToken = 1f; //攻击动作权重
		[LabelText("触发几率"), Range(0, 100), FoldoutGroup("Split/$actionTag", false)]
		public float chancePercentage = 75f;
		[LabelText("等待时间"), FoldoutGroup("Split/$actionTag", false)]
		public float waitTime = 0.2f;
		[LabelText("等待朝向目标"), FoldoutGroup("Split/$actionTag", false)]
		public bool waitLockAtTarget = true; //等待旋转结束后执行动作
		[LabelText("朝向目标速度"), FoldoutGroup("Split/$actionTag", false)]
		public float lookAtRotationSpeed = 1080; //最大旋转速度增量
		[LabelText("朝向目标最大持续时间"), FoldoutGroup("Split/$actionTag", false)]
		public float maxLookAtRotationMaxTime = 0.5f; //最大旋转时间 与 旋转Epsilon之间任一满足则停止旋转
	}

	[Serializable]
	public class AICombatStateConfig{
		[LabelText("战斗状态")] [HideLabel, VerticalGroup("Split")]
		public AICombatStateType aiCombatStateType;
		[LabelText("触发几率"), Range(0, 100), FoldoutGroup("Split/$Name", false)]
		public float TriggerChancePercentage = 75;
		[LabelText("触发条件"), FoldoutGroup("Split/$Name", false)]
		public AIActionConditionSet ConditionSet;

		[LabelText("行为队列"), FoldoutGroup("Split/$Name", false)]
		public AIBehaviourSequence behaviourSequence;

		private string Name => aiCombatStateType.ToString();

		public AICombatStateConfig(){
		}

		public AICombatStateConfig(AICombatStateType aiCombatStateType){
			this.aiCombatStateType = aiCombatStateType;
			ConditionSet = new AIActionConditionSet();
			ConditionSet.Conditions.Add(new AIDistanceActionCondition());
			switch (aiCombatStateType){
				case AICombatStateType.MeleeCombat:
					ConditionSet.Conditions.Add(new AIFightCircleCondition(){circles = CircleType.Melee});
					break;
				case AICombatStateType.ChaseTarget:
					break;
				case AICombatStateType.RangedCombat:
					break;
				case AICombatStateType.StudyTarget:
					break;
				case AICombatStateType.Flee:
					break;
			}
		}
	}



	[Serializable]
	public class ActionChances{
		[LabelText("动作列表")]
		public List<EnemyAttackActionData> enemyAttackActionData;
	}

}
