using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Combat.Runtime{
	[Serializable]
	public class AIActionConditionSet{
		[SerializeReference]
		public List<AIActionCondition> Conditions = new List<AIActionCondition>();

		/// <summary>
		/// 条件集开始监听
		/// </summary>
		/// <param name="aiController"></param>
		public void Start(EnemyController aiController){
			for (int i = 0; i < Conditions.Count; i++){
				Conditions[i].Start(aiController);
			}
		}

		/// <summary>
		/// 当条件被满足并且被触发
		/// </summary>
		public void OnActionTriggered(EnemyController aiController){
			for (int i = 0; i < Conditions.Count; i++){
				Conditions[i].OnActionTriggered(aiController);
			}
		}

		/// <summary>
		/// 条件被判断为已满足
		/// </summary>
		/// <param name="aiController"></param>
		public void OnConditionSetIsMet(EnemyController aiController){
			for (int i = 0; i < Conditions.Count; i++){
				Conditions[i].OnConditionIsMet(aiController);
			}
		}

		public bool IsConditionMet(EnemyController aiController){
			bool isMet = true;
			for (int i = 0; i < Conditions.Count; i++){
				if (!Conditions[i].IsConditionMet(aiController)){
					isMet = false;
					break;
				}
			}
			return isMet;
		}

		/// <summary>
		/// 条件集停止监听
		/// 角色退出战斗时触发
		/// </summary>
		public void Stop(){
			for (int i = 0; i < Conditions.Count; i++){
				Conditions[i].Stop();
			}
		}
	}
}
