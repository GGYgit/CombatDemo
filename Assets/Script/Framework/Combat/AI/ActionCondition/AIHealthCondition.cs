using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 血量条件
	/// </summary>
	public class AIHealthCondition : AIActionCondition{
		[LabelText("比较类型")]
		public EConditionType conditionType = EConditionType.Equal;
		[LabelText("血量百分比"), Range(0, 100)]
		public float targetValue = 50f;
		[LabelText("相等宽容值"), ShowIf("conditionType", EConditionType.Equal)]
		public float nearlyEqualAcceptance = 1f;

		/// <summary>
		/// todo 属性判断
		/// </summary>
		/// <param name="aiController"></param>
		/// <returns></returns>
		public override bool IsConditionMet(EnemyController aiController){

			// float healthRatio = aiController.ownerCharacter.GetHealthRatio() * 100;
			// switch (conditionType){
			// 	case EConditionType.LessThan:
			// 		return healthRatio < targetValue;
			// 	case EConditionType.HigherThan:
			// 		return healthRatio > targetValue;
			// 	case EConditionType.Equal:
			// 		return Math.Abs(healthRatio - targetValue) <= nearlyEqualAcceptance;
			// }
			return false;
		}
	}
}
