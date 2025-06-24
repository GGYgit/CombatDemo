using UnityEngine;

namespace Framework.Combat.Runtime{
	public static class LayerManager{
		public static int DefaultIndex = LayerMask.NameToLayer("Default");
		public static LayerMask Default = 1 << DefaultIndex;
		public static int PlayerIndex = LayerMask.NameToLayer("Player");
		public static LayerMask Player = 1 << PlayerIndex;

		public static int TriggerIndex = LayerMask.NameToLayer("Trigger");
		public static LayerMask Trigger = 1 << TriggerIndex;

		public static int HitBoxIndex = LayerMask.NameToLayer("HitBox");
		public static LayerMask HitBox = 1 << HitBoxIndex;

		public static int EnemyIndex = LayerMask.NameToLayer("Enemy");
		public static LayerMask Enemy = 1 << EnemyIndex;

		public static int IngoreIndex = LayerMask.NameToLayer("Ignore Raycast");
		public static LayerMask Ignore = 1 << IngoreIndex;

		public static int UIIndex = LayerMask.NameToLayer("UI");
		public static LayerMask UI = 1 << UIIndex;

		public static int ProjectileIndex = LayerMask.NameToLayer("Projectile");
		public static LayerMask Projectile = 1 << ProjectileIndex;


		public static LayerMask DamageDetection = Default | Player | Enemy ;
		public static LayerMask EnemySightDetectionIgnore = UI | Projectile | Enemy;

		public static LayerMask Character = Player | Enemy ;
		public static LayerMask Obstacles = Default ;


	}
}
