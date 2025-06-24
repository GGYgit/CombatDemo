using System;
using UnityEngine;

namespace Framework.Combat.Runtime{
	[Serializable]
	public class ProjectileDetector{
		public float radius;
		private Vector3 lastPosition;
		private Projectile projectile;

		public void Activate(Projectile projectile){
			this.projectile = projectile;
			lastPosition = projectile.transform.position;
		}

		public void Deactivate(){
		}

		public virtual bool CheckHit(out HitInfo hitInfo,out Collider hitCol){
			hitInfo = null;
			hitCol = null;
			Vector3 current = projectile.transform.position;
			Vector3 dir = current - lastPosition;
			float dist = dir.magnitude;
			if (Physics.SphereCast(lastPosition, radius, dir.normalized, out RaycastHit raycastHit, dist,
				    LayerManager.DamageDetection)){
				hitInfo = new HitInfo(-1, raycastHit.point);
				hitCol = raycastHit.collider;
				var combatEntity = hitCol.GetComponent<ICombatEntity>();
				if (combatEntity != null){
					if (combatEntity.GetEntityCombatTeam().IsEnemyTeam(projectile.CombatTeamType) && combatEntity.CheckCanTakeDamage()){
						return true;
					}
					return false;
				}
				return true;
			}
			lastPosition = current;
			return false;
		}
	}
}
