using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 角色组件基类，角色组件指动作、运动 、属性计算等概念的组件
	/// </summary>
	public abstract class CharacterComponent : MonoBehaviour{
		protected BaseCharacter owner;

		public BaseCharacter Owner{
			get{ return owner; }
		}

		public void Init(BaseCharacter character){
			owner = character;
			OnInit(owner);
		}

		protected virtual void OnInit(BaseCharacter character){
		}
	}
}
