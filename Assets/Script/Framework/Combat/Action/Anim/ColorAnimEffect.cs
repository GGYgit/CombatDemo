using UnityEngine;

namespace Framework.Combat.Runtime{
	public class ColorAnimEffect : IGeometryAnimEffect{
		public Color color = Color.red;
		public string headName = "Head";
		public float duration = 0.5f;
		private Color originalColor;
		private Material material;
		public float Duration => duration;

		public void Initialize(Transform root){
			if (material != null) return;
			Transform headTran = root.FindChildByNameRecursive(headName);
			material = headTran.GetComponent<Renderer>().material;
			originalColor = material.color;
		}

		public void Evaluate(float time){
			if (material == null) return;
			material.color = color;
		}

		public void Restore(){
			if (material == null) return;
			material.color = originalColor;
		}
	}
}
