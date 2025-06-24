using System;
using UnityEngine;

namespace Framework.Combat.Runtime{
	[DefaultExecutionOrder(-1000)]
	public class GameEntry : MonoBehaviour{
		public static MainComponent Main{ get; private set; }
		public static CombatTimeManagerComponent Time{ get; private set; }
		public static EffectManagerComponent Effect{ get; private set; }
		public static EnemyAggressiveComponent Aggressive{ get; private set; }

		/// <summary>
		/// 组件缓存
		/// </summary>
		private void Start(){
			Main = GetComponent<MainComponent>();
			Time = GetComponent<CombatTimeManagerComponent>();
			Effect = GetComponent<EffectManagerComponent>();
			Aggressive = GetComponent<EnemyAggressiveComponent>();

		}

		/// <summary>
		/// 获取游戏框架组件。
		/// </summary>
		/// <typeparam name="T">要获取的游戏框架组件类型。</typeparam>
		/// <returns>要获取的游戏框架组件。</returns>
		public new static T GetComponent<T>() where T : FrameworkComponent{
			return (T) FrameworkComponentContainer.GetComponent(typeof(T));
		}

		/// <summary>
		/// 获取游戏框架组件。
		/// </summary>
		/// <param name="type">要获取的游戏框架组件类型。</param>
		/// <returns>要获取的游戏框架组件。</returns>
		public new static FrameworkComponent GetComponent(Type type){
			return FrameworkComponentContainer.GetComponent(type);
		}
	}
}
