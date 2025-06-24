using System;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 玩家目标组件，管理玩家当前目标
	/// </summary>
	[Serializable]
	public class PlayerTargetManagerComponent : TargetManagerComponent{
		/// <summary>
		/// 玩家目标将返回基于屏幕中心优先的角色
		/// </summary>
		public override BaseCharacter GetNearestTarget(float range){
			var targets = owner.GetPossibleTargets(range);
			if (targets.Count > 0) return targets[0];
			return null;
		}
	}
}
