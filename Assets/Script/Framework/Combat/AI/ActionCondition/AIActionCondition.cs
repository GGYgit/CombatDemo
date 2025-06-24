using System;

namespace Framework.Combat.Runtime{
	[Serializable]
	public abstract class AIActionCondition{
		/// <summary>
		/// 启用条件
		/// </summary>
		public virtual void Start(EnemyController aiController){
		}

		/// <summary>
		/// 当条件被满足，并且已被触发
		/// </summary>
		public virtual void OnActionTriggered(EnemyController aiController){
		}


		/// <summary>
		/// 当条件被满足，并且执行过一次判断
		/// </summary>
		public virtual void OnConditionIsMet(EnemyController aiController){
		}



		/// <summary>
		/// 停止条件
		/// </summary>
		public virtual void Stop(){
		}

		/// <summary>
		/// 条件是否满足
		/// </summary>
		/// <param name="aiController"></param>
		/// <returns></returns>
		public abstract bool IsConditionMet(EnemyController aiController);
	}
}
