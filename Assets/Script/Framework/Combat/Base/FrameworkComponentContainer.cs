using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Combat.Runtime{
	public sealed class FrameworkComponentContainer{
		private static readonly LinkedList<FrameworkComponent> s_CombatGlobalComponents = new();

		/// <summary>
		/// 获取战斗框架组件。
		/// </summary>
		/// <typeparam name="T">要获取的战斗框架组件类型。</typeparam>
		/// <returns>要获取的战斗框架组件。</returns>
		public static T GetComponent<T>() where T : FrameworkComponent{
			return (T) GetComponent(typeof(T));
		}

		/// <summary>
		/// 获取战斗框架组件。
		/// </summary>
		/// <param name="type">要获取的战斗框架组件类型。</param>
		/// <returns>要获取的战斗框架组件。</returns>
		public static FrameworkComponent GetComponent(Type type){
			var current = s_CombatGlobalComponents.First;
			while (current != null){
				if (current.Value.GetType() == type) return current.Value;
				current = current.Next;
			}
			return null;
		}

		/// <summary>
		/// 获取战斗框架组件。
		/// </summary>
		/// <param name="typeName">要获取的战斗框架组件类型名称。</param>
		/// <returns>要获取的战斗框架组件。</returns>
		public static FrameworkComponent GetComponent(string typeName){
			var current = s_CombatGlobalComponents.First;
			while (current != null){
				var type = current.Value.GetType();
				if (type.FullName == typeName || type.Name == typeName) return current.Value;
				current = current.Next;
			}
			return null;
		}

		/// <summary>
		/// 关闭战斗框架。
		/// </summary>
		/// <param name="shutdownType">关闭战斗框架类型。</param>
		public static void Shutdown(){
			Debug.Log("Shutdown Game Framework...");
			var mainComponent = GetComponent<MainComponent>();
			if (mainComponent != null){
				mainComponent.Shutdown();
				mainComponent = null;
			}
			s_CombatGlobalComponents.Clear();
		}

		/// <summary>
		/// 注册框架组件。
		/// </summary>
		/// <param name="CombatGlobalComponent">要注册的框架组件。</param>
		internal static void RegisterComponent(FrameworkComponent CombatGlobalComponent){
			if (CombatGlobalComponent == null){
				Debug.LogError("Game Framework component is invalid.");
				return;
			}
			var type = CombatGlobalComponent.GetType();
			var current = s_CombatGlobalComponents.First;
			while (current != null){
				if (current.Value.GetType() == type){
					Debug.LogError(
						string.Format("Game Framework component type '{0}' is already exist.", type.FullName));
					return;
				}
				current = current.Next;
			}
			s_CombatGlobalComponents.AddLast(CombatGlobalComponent);
		}
	}
}
