using UnityEngine;

namespace Framework.Combat.Runtime{
	[System.Serializable]
	public abstract class BaseCircle: MonoBehaviour{
		[Range(60f, 360f)]
		public float degrees = 360f;
		[Range(0.5f, 10f)]
		public float radius;
		[SerializeField]
		public float slotsWeightCapacity;//最大slot
		public int maximumSlots = 7;
		public FightingNode[] fightingNodes;

		protected float slotsWeight;//当前slot

		private void Start(){
			ResetAvailableSlotsWeight();
			fightingNodes = new FightingNode[maximumSlots];
			for (int i = 0; i < maximumSlots; i++){
				fightingNodes[i] = new FightingNode{
					index = i,
					state = NodeState.Valid
				};
			}
		}

		/// <summary>
		/// 有效性检测，通常是无法直线到达玩家处则判断为无效圈
		/// </summary>
		public virtual void UpdateNodeValidity(){
		}

		protected Vector3 ComputePosition(float degrees, float radius){
			return new Vector3
			(Mathf.Cos(Mathf.Deg2Rad * degrees) * radius
				, 0
				, Mathf.Sin(Mathf.Deg2Rad * degrees) * radius);
		}

		public float GetAvailableSlotsWeight(){
			return slotsWeight;
		}

		public float GetCapacitySlotsWeight(){
			return slotsWeightCapacity;
		}

		public bool UseAvailSlotsWeight(float weight){
			if (slotsWeight < weight){
				return false;
			}
			slotsWeight -= weight;
			return true;
		}

		public void SetAvailableSlotsWeight(float weight){
			slotsWeight = Mathf.Clamp(weight, 0f, slotsWeightCapacity);
		}

		public void ResetAvailableSlotsWeight(){
			slotsWeight = slotsWeightCapacity;
		}

		public abstract bool Register(FightCircleSubject enemy);
		public abstract bool Unregister(FightCircleSubject enemy);
		public abstract bool IsContains(FightCircleSubject enemy);
		public abstract CircleType GetCircleType();
		public abstract void ClearAll();
	}
}
