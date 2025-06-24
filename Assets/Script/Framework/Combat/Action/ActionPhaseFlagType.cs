using System;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 动作阶段标记
	/// </summary>
	[Flags]
	public enum ActionPhaseFlagType{
		None = 0,
		Continue = 1 << 1, //可进入下一连招
		Interruptible = 1 << 2, //可被移动输入打断
	}
}
