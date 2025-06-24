using UnityEngine;

namespace Framework.Combat.Runtime{
	public class ImpactPiece : MonoBehaviour{
		private Rigidbody rb;
		private float lifetime = 1f;
		private float fadeTime = 0.5f;
		private float timer;

		private Material mat;
		private Color originalColor;

		void Awake(){
			rb = GetComponent<Rigidbody>();
			mat = GetComponent<Renderer>().material;
			originalColor = mat.color;
		}

		public void Init(Vector3 force, float lifetime = 2f){
			rb.velocity = force;
			this.lifetime = lifetime;
			timer = 0f;
			mat.color = originalColor;
			gameObject.SetActive(true);
		}

		void Update(){
			timer += Time.deltaTime;
			if (timer > lifetime - fadeTime){
				float t = 1 - (timer - (lifetime - fadeTime)) / fadeTime;
				Color faded = originalColor;
				faded.a = Mathf.Clamp01(t);
				mat.color = faded;
			}
			if (timer >= lifetime){
				Destroy(gameObject); // Demo不用搞池子了
			}
		}
	}
}
