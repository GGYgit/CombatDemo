using UnityEngine;

namespace Framework.Combat.Runtime{
	public class Projectile : MonoBehaviour{
		public float speed = 25;
		public ProjectileMover mover;
		public DamageConfig damageConfig;
		public ProjectileDetector detector;
		public float maxLifetime = 5f;


		private DamageComponent damageComp;
		private float lifeTimer = 0f;
		private BaseCharacter launcher;
		private bool active;


		public CombatTeamType CombatTeamType{get;set;}

		public void Launch(BaseCharacter launcher, Vector3 direction, float speedFactor){
			this.launcher = launcher;
			CombatTeamType = this.launcher.GetEntityCombatTeam();
			damageComp = launcher.GetComponent<DamageComponent>();
			active = true;
			mover.Init(this, direction, speed * speedFactor);
			detector.Activate(this);
		}


		void Update(){
			if (!active) return;
			lifeTimer += Time.deltaTime;
			if (lifeTimer >= maxLifetime){
				Deactivate();
				return;
			}
			mover.Move(Time.deltaTime);
			if (detector.CheckHit(out HitInfo hitInfo, out Collider hitCol)){
				hitInfo.damageConfig = damageConfig;
				damageComp.HitGameObject(hitInfo, hitCol);
				Deactivate();
			}
		}

		public void Deactivate(){
			active = false;
			detector.Deactivate();
			Destroy(gameObject);
		}
	}
}
