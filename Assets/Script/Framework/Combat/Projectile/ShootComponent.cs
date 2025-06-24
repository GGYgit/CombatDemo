using UnityEngine;

namespace Framework.Combat.Runtime{
	public class ShootComponent : CharacterComponent{
		public Transform firePoint;

		public void Fire(Projectile projectile, Vector3 direction, float speedFactor){
			Projectile proj = Instantiate(projectile, firePoint.position, Quaternion.LookRotation(direction));
			proj.Launch(owner, direction, speedFactor);
		}
	}
}
