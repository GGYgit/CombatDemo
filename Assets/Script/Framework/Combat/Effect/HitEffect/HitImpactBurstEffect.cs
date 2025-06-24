using UnityEngine;

namespace Framework.Combat.Runtime{
	public class HitImpactBurstEffect : BaseHitEffect{
		public GameObject piecePrefab;
		public int count = 12;
		public float speed = 4f;
		public float radius = 0.3f;
		public Vector2 lifecycle = new Vector2(0.5f, 1f);

		public override void Init(HitEffectComponent comp){
		}

		public override void Shutdown(){
		}

		public override void Trigger(DamageInfo damageInfo){
			for (int i = 0; i < count; i++){
				Vector3 dir = Random.onUnitSphere;
				Vector3 pos = damageInfo.hitPosition + dir * Random.Range(0f, radius);
				GameObject obj = GameObject.Instantiate(piecePrefab, pos, Quaternion.identity);
				ImpactPiece piece = obj.GetComponent<ImpactPiece>();
				piece.Init(dir * speed, Random.Range(lifecycle.x, lifecycle.y));
			}
		}

		public override void Tick(float deltaTime){
		}
	}
}
