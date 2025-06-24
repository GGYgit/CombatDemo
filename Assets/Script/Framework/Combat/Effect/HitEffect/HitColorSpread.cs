using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 受击颜色扩散效果
	/// </summary>
	[Serializable]
	public class HitColorSpread : BaseHitEffect{
		[Header("效果设置")]
		public Color hitColor = Color.red;
		public float maxRadius = 2f;
		public float fadeSpeed = 3f;
		public float hitDuration = 0.3f;
		[Range(0, 1)]
		public float maxIntensity = 0.8f;


		public Renderer renderer;
		private Material hitMaterial;
		private float currentRadius;
		private float hitTimer;
		private bool isHit;
		private Transform ownerTransform;

		public override void Trigger(DamageInfo damageInfo){
			Vector3 localHitPosition = ownerTransform.InverseTransformPoint(damageInfo.hitPosition);
			hitMaterial.SetVector("_HitPosition", localHitPosition);
			currentRadius = 0;
			hitTimer = 0;
			isHit = true;
		}

		public override void Init(HitEffectComponent comp){
			if (renderer == null) return;
			ownerTransform = comp.transform;
			hitMaterial = renderer.material;
			// hitMaterial.SetColor("_MainColor", hitMaterial.color);
			hitMaterial.SetColor("_Color", hitMaterial.color);
			hitMaterial.SetColor("_HitColor", hitColor);
			hitMaterial.SetFloat("_HitRadius", 0);
			hitMaterial.SetFloat("_HitIntensity", 0);
		}

		public override void Shutdown(){
			if (hitMaterial != null)
				Object.Destroy(hitMaterial);
		}

		public override void Tick(float deltaTime){
			if (isHit){
				hitTimer += Time.deltaTime;
				if (hitTimer <= hitDuration){
					// 扩散阶段：半径增大，强度增强
					currentRadius = Mathf.Lerp(0, maxRadius, hitTimer / hitDuration);
					float currentIntensity = Mathf.Lerp(0, maxIntensity, hitTimer / hitDuration);
					hitMaterial.SetFloat("_HitRadius", currentRadius);
					hitMaterial.SetFloat("_HitIntensity", currentIntensity);
				} else{
					// 淡出阶段：保持半径，减弱强度
					float fadeValue = Mathf.Lerp(maxIntensity, 0, (hitTimer - hitDuration) * fadeSpeed);
					hitMaterial.SetFloat("_HitIntensity", fadeValue);
					if (fadeValue <= 0.01f){
						isHit = false;
						ResetEffect();
					}
				}
			}
		}

		void ResetEffect(){
			hitMaterial.SetFloat("_HitRadius", 0);
			hitMaterial.SetFloat("_HitIntensity", 0);
		}
	}
}
