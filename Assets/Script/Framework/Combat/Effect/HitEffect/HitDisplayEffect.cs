using UnityEngine;

namespace Framework.Combat.Runtime{
	public class HitDisplayEffect : BaseHitEffect{
		public string[] transformNames;
		public bool display = false;
		private Transform root;

		public override void Trigger(DamageInfo damageInfo){
			foreach (var transformName in transformNames){
				Transform tran = root.FindChildByNameRecursive(transformName);
				if (tran){
					tran.gameObject.SetActive(display);
				}
			}
		}

		public override void Init(HitEffectComponent comp){
			root = comp.transform;
		}

		public override void Shutdown(){
		}

		public override void Tick(float deltaTime){
		}
	}
}
