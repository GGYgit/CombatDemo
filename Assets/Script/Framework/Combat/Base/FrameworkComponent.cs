using UnityEngine;

namespace Framework.Combat.Runtime{
	public abstract class FrameworkComponent:MonoBehaviour{
		/// <summary>
		/// 游戏框架组件初始化。
		/// 重写时必要调用
		/// </summary>
		protected virtual void Awake(){
			FrameworkComponentContainer.RegisterComponent(this);
		}
	}
}
