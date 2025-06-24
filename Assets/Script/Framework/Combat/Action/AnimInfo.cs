using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Combat.Runtime{
	[Serializable]
	public class AnimInfo{
		public string name;
		public float time;
		[SerializeReference,OnValueChanged("CalculateTime",true)]
		public List<IGeometryAnimEffect> effects = new List<IGeometryAnimEffect>();

		public override string ToString(){
			return name;
		}
#if UNITY_EDITOR
		private void CalculateTime(){
			float t = 0;
			foreach (var animEffect in effects){
				if (animEffect.Duration > t) t = animEffect.Duration;
			}
			time = t;
		}
#endif
	}
}
