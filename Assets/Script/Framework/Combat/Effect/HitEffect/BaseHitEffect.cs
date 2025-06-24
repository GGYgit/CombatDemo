using System;

namespace Framework.Combat.Runtime{
	[Serializable]
	public abstract class BaseHitEffect{
		public abstract void Init(HitEffectComponent comp);
		public abstract void Shutdown();
		public abstract void Trigger(DamageInfo damageInfo);
		public abstract void Tick(float deltaTime);
	}
}
