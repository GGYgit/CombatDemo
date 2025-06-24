using System;
using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 感知基类
	/// </summary>
	[Serializable]
	public abstract class AbstractAIPerception{
		public bool drawGizmos = false;
		protected AIPerceptionComponent owner;
		public float tickInterval = 1f;
		protected float cooldown;

		public virtual void Start(AIPerceptionComponent perceptionComp){
			this.owner = perceptionComp;
			cooldown = tickInterval;
		}

		public void OnTick(float deltaTime){
			cooldown += deltaTime;
			if (cooldown >= tickInterval){
				Tick(cooldown);
				cooldown = 0;
			}
		}

		public virtual void Tick(float deltaTime){
		}

		/// <summary>
		/// 感知到目标时
		/// </summary>
		public virtual void OnPerceive(GameObject returnedObject){
			owner.InvokeOnPerception(this, returnedObject);
		}

		public virtual void OnLostTarget(){
			owner.InvokeOnLostTarget(this);
		}

		public virtual void Stop(){
		}

		public virtual void OnGizmos(GameObject gameObject){
		}
	}
}
