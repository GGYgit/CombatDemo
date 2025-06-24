#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Framework.Combat.Runtime{
	public static class ScriptableObjectUtility{
		public static T CreateAsset<T>() where T : ScriptableObject{
			var asset = ScriptableObject.CreateInstance<T>();
			ProjectWindowUtil.CreateAsset(asset, "New " + typeof(T).Name + ".asset");
			return asset;
		}
	}
}
#endif
