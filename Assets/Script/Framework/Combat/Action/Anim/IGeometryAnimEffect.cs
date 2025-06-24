using UnityEngine;

namespace Framework.Combat.Runtime{
	public interface IGeometryAnimEffect{
		void Initialize(Transform root);
		void Evaluate(float time);
		void Restore();

		float Duration { get; }
	}
}
