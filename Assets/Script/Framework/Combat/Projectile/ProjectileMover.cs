using System;
using UnityEngine;

namespace Framework.Combat.Runtime{
	[Serializable]
	public class ProjectileMover{
		private Vector3 velocity;
		private Projectile projectile;

		public void Init(Projectile projectile, Vector3 direction, float speed){
			this.projectile = projectile;
			velocity = direction.normalized * speed;
		}

		public void Move(float dt){
			projectile.transform.position += velocity * dt;
		}
	}
}
