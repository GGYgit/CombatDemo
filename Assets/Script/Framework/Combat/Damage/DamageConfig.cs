using System;

namespace Framework.Combat.Runtime{
	[Serializable]
	public class DamageConfig{
		public float damageFactor = 1; //伤害系数
		public int freezeFrame = 3; //冻结帧
		public int hitReactionLevel = 1; //受击反馈级别
		public float knockBackDistance = 0; //击退距离
		public int cameraShakeLevel = 1; //命中摄像机震动级别
	}
}
