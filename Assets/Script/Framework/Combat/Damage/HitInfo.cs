using UnityEngine;

namespace Framework.Combat.Runtime{
	public class HitInfo{
		public int hitId;//Hit Id仅在>=0时记录
		public Vector3 hitPosition;
		public DamageConfig damageConfig;

		public HitInfo(){
		}

		public HitInfo(int hitId, Vector3 hitPosition){
			this.hitId = hitId;
			this.hitPosition = hitPosition;
		}

		public HitInfo(int hitId, Vector3 hitPosition, DamageConfig damageConfig){
			this.hitId = hitId;
			this.hitPosition = hitPosition;
			this.damageConfig = damageConfig;
		}
	}
}
