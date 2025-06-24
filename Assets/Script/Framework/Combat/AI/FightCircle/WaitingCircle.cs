using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Framework.Combat.Runtime{
	public class WaitingCircle
		: BaseCircle{
		[ShowInInspector] private Dictionary<int, FightCircleSubject> engagedSlots;

		private void Awake(){
			engagedSlots = new Dictionary<int, FightCircleSubject>();
		}


		public override CircleType GetCircleType(){
			return CircleType.Wait;
		}

		public override bool IsContains(FightCircleSubject enemy){
			return engagedSlots.ContainsKey(enemy.GetInstanceID());
		}

		public override bool Register(FightCircleSubject enemy){
			int instance = enemy.GetInstanceID();
			engagedSlots.Add(instance
				, enemy);
			return true;
		}

		public override bool Unregister(FightCircleSubject enemy){
			bool isContains = IsContains(enemy);
			int instance = enemy.GetInstanceID();
			if (isContains){
				engagedSlots.Remove(instance);
			}
			return isContains;
		}

		public override void ClearAll(){
			engagedSlots.Clear();
		}
	}
}
