using System;

namespace Framework.Combat.Runtime{
	[Serializable]
	public abstract class BaseActionEvent{
		public float startTime;
		public float length;
		public float EndTime => startTime + length;

		/// <summary>
		/// 动画开始时
		/// </summary>
		/// <param name="action"></param>
		public virtual void OnActionStarted(BaseAction performingAction){}
		/// <summary>
		/// 动作结束时
		/// </summary>
		/// <param name="action"></param>
		public virtual void OnActionEnded(BaseAction performingAction){}

		/// <summary>
		/// 该事件开始时
		/// </summary>
		/// <param name="performingAction"></param>
		public virtual void OnEventStarted(BaseAction performingAction){
		}
		/// <summary>
		/// 该事件结束时
		/// </summary>
		/// <param name="performingAction"></param>
		public virtual void OnEventEnded(BaseAction performingAction){
		}

		/// <summary>
		/// 只在事件激活时Tick
		/// </summary>
		/// <param name="deltaTime"></param>
		public virtual void OnTick(float deltaTime){}
	}
}
