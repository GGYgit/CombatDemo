namespace Framework.Combat.Runtime{
	/// <summary>
	/// 对应战斗圈内是否存在该角色
	/// </summary>
	public class AIFightCircleCondition : AIActionCondition{
		public CircleType circles = CircleType.Melee;

		public override bool IsConditionMet(EnemyController aiController){
			FightingCircle fightingCircle = aiController.GetTargetFightCircle();
			return fightingCircle.GetCircle(circles).IsContains(aiController);
		}
	}
}
