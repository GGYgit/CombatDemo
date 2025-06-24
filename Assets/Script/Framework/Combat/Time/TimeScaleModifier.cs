using System;

namespace Framework.Combat.Runtime{
	[Serializable]
	public class TimeScaleModifier{
		public string id;
		public float timescale;
		public Action<float> OnTick;
		public ETimeScaleEffectTargetType timeScaleEffectTargetType =
			ETimeScaleEffectTargetType.Enemy | ETimeScaleEffectTargetType.Player;
	}

	[Flags]
	public enum ETimeScaleEffectTargetType{
		Player = 1 << 1,
		Enemy = 1 << 2,
		All = Player | Enemy,
	}
}
