using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	///战斗圈订阅方接口
	/// </summary>
	public interface FightCircleSubject{

		public float GetSlotWeight(){
			return 1f;
		}

		public int GetInstanceID();

		public Vector3 GetPosition();

		public CombatStyle GetCombatStyle();

		public void SetCircleType(CircleType circle){}

		public bool CanAttack();

		public float GetFightPriority();

		public void OnCircleUpdate(CircleType circle);
		Transform transform{ get; }
		GameObject gameObject{ get; }
	}
}
