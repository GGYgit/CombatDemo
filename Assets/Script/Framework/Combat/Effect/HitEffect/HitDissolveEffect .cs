using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 受击颜色扩散效果
	/// </summary>
	[Serializable]
	public class HitDissolveEffect : BaseHitEffect{
		public Renderer renderer;
		public float hitDuration = 2f;
		public float dissolveRadius = 0.2f;
		public Color dissolveEdgeColor = Color.red;
		public float dissolveEdgeWidth = 0.05f;

		private bool isActive = false;
		private Material hitMaterial;
		private float dissolveAmount;
		private float hitTimer;
		private Transform ownerTransform;

		public override void Init(HitEffectComponent comp){
			if (renderer == null) return;
			hitMaterial = renderer.material;
			ownerTransform = comp.transform;
		}

		public override void Trigger(DamageInfo damageInfo){
			Vector3 localHitPosition = ownerTransform.InverseTransformPoint(damageInfo.hitPosition);
			hitMaterial.SetVector("_HitPosition", localHitPosition);
			ResetEffect();
			dissolveAmount = 1;
			isActive = true;
			hitTimer = 0;
			UpdateMaterial();
		}


		public override void Shutdown(){
			ResetEffect();
		}

		public override void Tick(float deltaTime){
			if (!isActive) return;
			hitTimer += Time.deltaTime;
			dissolveAmount = Mathf.Lerp(1, 0, hitTimer / hitDuration);
			if (dissolveAmount <= 0f){
				dissolveAmount = 0f;
				isActive = false;
			}
			UpdateMaterial();
		}

		void UpdateMaterial(){
			hitMaterial.SetFloat("_DissolveAmount", dissolveAmount);
		}

		public void ResetEffect(){
			dissolveAmount = 1;
			isActive = false;
			hitMaterial.SetFloat("_DissolveRadius", dissolveRadius);
			hitMaterial.SetFloat("_DissolveEdgeWidth", dissolveEdgeWidth);
			hitMaterial.SetColor("_DissolveEdgeColor", dissolveEdgeColor);
			UpdateMaterial();
		}
	}
}
