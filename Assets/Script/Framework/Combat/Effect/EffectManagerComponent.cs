using System.Collections.Generic;
using UnityEngine;

namespace Framework.Combat.Runtime{
	public class EffectManagerComponent : FrameworkComponent{
		[System.Serializable]
		public class EffectEntry{
			public string name;
			public GameObject prefab;
		}

		[Header("预加载的特效表")]
		public List<EffectEntry> effectList = new List<EffectEntry>();

		private Dictionary<string, GameObject> prefabDict = new Dictionary<string, GameObject>();
		private Dictionary<string, Queue<GameObject>> poolDict = new Dictionary<string, Queue<GameObject>>();

		protected override void Awake(){
			base.Awake();
			foreach (var entry in effectList){
				if (!prefabDict.ContainsKey(entry.name)){
					prefabDict.Add(entry.name, entry.prefab);
					poolDict.Add(entry.name, new Queue<GameObject>());
				}
			}
		}

		/// <summary>
		/// 一次性播放粒子特效
		/// </summary>
		public GameObject PlayOnceEffect(string name, Vector3 position, Quaternion rotation){
			GameObject effect = PlayEffect(name, position, rotation, null, 0f);
			float duration = GetEffectDuration(effect);
			StartCoroutine(AutoRecycle(effect, name, duration));
			return effect;
		}


		public GameObject PlayLoopEffect(string name, Transform followTarget, Vector3 localOffset,
			Quaternion rotationOffset){
			GameObject effect = PlayEffect(name, followTarget.position + localOffset, rotationOffset, followTarget, 0f);

			// 添加跟随组件（自动更新位置）
			var follow = effect.GetComponent<EffectFollowTarget>();
			if (follow == null)
				follow = effect.AddComponent<EffectFollowTarget>();
			follow.target = followTarget;
			follow.offset = localOffset;
			follow.rotationOffset = rotationOffset;
			return effect;
		}

		public void StopLoopEffect(GameObject effect, string name){
			if (effect == null) return;
			var follow = effect.GetComponent<EffectFollowTarget>();
			if (follow != null)
				Destroy(follow);
			RecycleEffect(effect, name);
		}

		/// <summary>
		/// 播放特效
		/// </summary>
		public GameObject PlayEffect(string name, Vector3 position, Quaternion rotation, Transform parent = null,
			float autoDestroyTime = 2f){
			if (!prefabDict.ContainsKey(name)){
				Debug.LogWarning($"特效 [{name}] 未注册");
				return null;
			}
			GameObject effect = null;
			Queue<GameObject> pool = poolDict[name];
			if (pool.Count > 0){
				effect = pool.Dequeue();
				effect.SetActive(true);
			} else{
				effect = Instantiate(prefabDict[name]);
			}
			if (parent != null){
				effect.transform.SetParent(parent);
			}
			effect.transform.position = position;
			effect.transform.rotation = rotation;
			if (autoDestroyTime > 0){
				StartCoroutine(AutoRecycle(effect, name, autoDestroyTime));
			}
			return effect;
		}


		private System.Collections.IEnumerator AutoRecycle(GameObject obj, string name, float delay){
			yield return new WaitForSeconds(delay);
			if (obj != null){
				RecycleEffect(obj, name);
			}
		}

		private float GetEffectDuration(GameObject obj){
			float maxDuration = 0f;
			var particleSystems = obj.GetComponentsInChildren<ParticleSystem>();
			foreach (var ps in particleSystems){
				var main = ps.main;
				float duration = main.duration + main.startLifetime.constantMax;
				if (duration > maxDuration)
					maxDuration = duration;
			}

			// fallback 1秒
			return Mathf.Max(maxDuration, 1f);
		}

		private void RecycleEffect(GameObject obj, string name){
			obj.SetActive(false);
			obj.transform.SetParent(transform);
			if (poolDict.ContainsKey(name)){
				poolDict[name].Enqueue(obj);
			} else{
				Destroy(obj);
			}
		}
	}
}
