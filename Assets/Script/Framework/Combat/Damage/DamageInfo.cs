using UnityEngine;

namespace Framework.Combat.Runtime{
	public class DamageInfo{
		public float damageValue = 0;
		public ICombatEntity damageCauser; //伤害造成方
		public ICombatEntity damageReceiver; //伤害接受方
		public Vector3 hitPosition; //命中点
		public bool ignoreDefense = false; //忽略防御
		public int hitReactionLevel = 1;//受击反馈级别
		public int freezeFrame = 5; //冻结帧数
		public int cameraShakeLevel; //摄像机震动幅度
		public float knockBackDistance; //击退距离


		public static DamageInfo Create(){
			return new DamageInfo();
		}

		public static DamageInfo Create(DamageConfig damageConfig){
			DamageInfo damageInfo = new DamageInfo();
			damageInfo.hitReactionLevel = damageConfig.hitReactionLevel;
			damageInfo.freezeFrame = damageConfig.freezeFrame;
			damageInfo.knockBackDistance = damageConfig.knockBackDistance;
			damageInfo.cameraShakeLevel = damageConfig.cameraShakeLevel;
			return damageInfo;
		}
	}
}
