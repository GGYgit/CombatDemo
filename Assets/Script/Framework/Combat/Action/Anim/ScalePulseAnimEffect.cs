using UnityEngine;

namespace Framework.Combat.Runtime{
	public class ScalePulseAnimEffect : IGeometryAnimEffect{
		public string transformName = "Body";
		public float duration = 0.3f;
		public Vector3 scale = new Vector3(0.7f, 1.2f, 1.4f);

		private Transform transform;
		private Vector3 originalScale;

		public float Duration => duration;

		public void Initialize(Transform root){
			transform = root.FindChildByNameRecursive(transformName);
			if (transform == null) return;
			originalScale = transform.localScale;
		}

		public void Evaluate(float time){
			if (transform == null) return;
			float squash = Mathf.Sin(duration * Mathf.PI); // t ∈ [0,1]
			// Vector3 localScale = Vector3.Lerp(Vector3.one, scale, squash);
			transform.localScale = originalScale*squash;
		}

		public void Restore(){
			if (transform == null) return;
			transform.localScale = originalScale;
		}
	}
}
