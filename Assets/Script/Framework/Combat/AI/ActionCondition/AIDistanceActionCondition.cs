using System;
using Sirenix.OdinInspector;

namespace Framework.Combat.Runtime{
	public enum EConditionType{
		[LabelText("小于")] LessThan = 0,
		[LabelText("大于")] HigherThan = 1,
		[LabelText("几乎相等")] Equal = 2,
	}

	/// <summary>
	/// 距离条件
	/// </summary>
	public class AIDistanceActionCondition : AIActionCondition{
		[LabelText("比较类型")]
		public EConditionType conditionType = EConditionType.Equal;
		[LabelText("距离")]
		public float distance;
		[LabelText("相等宽容值"), ShowIf("conditionType", EConditionType.Equal)]
		public float nearlyEqualAcceptance = 0.5f;

		public override bool IsConditionMet(EnemyController aiController){
			BaseCharacter target = aiController.GetTarget();
			if (target == null) return false;
			float distanceFromTarget = aiController.OwnerCharacter.CalculateDistanceBetweenCharactersExtents(target);
			switch (conditionType){
				case EConditionType.LessThan:
					return distanceFromTarget < distance;
				case EConditionType.HigherThan:
					return distanceFromTarget > distance;
				case EConditionType.Equal:
					return Math.Abs(distanceFromTarget - distance) <= nearlyEqualAcceptance;
			}
			return false;
		}
	}
}
