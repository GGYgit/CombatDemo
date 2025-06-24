using UnityEngine;

namespace Framework.Combat.Runtime{
	public class ShakeAnimEffect : IGeometryAnimEffect{
		public float shakeDuration = 0.5f;
		public float shakeHeight = 0.2f;
		public string bodyTransformName = "Body";
		private float shakeTime = 0f;
		private Vector3 originalPos;
		private Transform body;
		public float Duration => shakeDuration;

		public void Initialize(Transform root){
			this.body = root.FindChildByNameRecursive(bodyTransformName);
			originalPos = body.localPosition;
			shakeTime = 0f;
		}

		public void Evaluate(float time){
			shakeTime = time;
			if (shakeTime < shakeDuration){
				float offset = Mathf.Sin(shakeTime * 40f) * shakeHeight;
				body.localPosition = originalPos + new Vector3(0, offset, 0);
			} else{
				body.localPosition = originalPos;
			}
		}

		public void Restore(){
			body.localPosition = originalPos;
		}
	}
}
