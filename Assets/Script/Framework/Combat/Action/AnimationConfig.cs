using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 武器攻击动画配置数据
	/// </summary>
	[Serializable]
	public class AnimationConfig{
		[Title("$animInfo")]
		public AnimInfo animInfo;
		[SerializeReference]
		public BaseActionEvent[] events = new BaseActionEvent[0];
	}



	public enum ActionPriority{
		None = 0,
		Low = 1,
		Medium = 2,
		High = 3,
		Highest = 4, //最高优先级，无视当前执行Action。需注意：最高优先级可以打断另一最高优先级的Action执行
	}
}
