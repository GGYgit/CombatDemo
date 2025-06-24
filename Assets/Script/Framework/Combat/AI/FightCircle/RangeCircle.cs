using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 远程圈，定义远程攻击的最小、最大范围
	/// </summary>
	public class RangeCircle
		: BaseCircle{
		public List<Vector3> positions = new List<Vector3>();
		public float maxRadius;

		[Header("Debug")]
		[ShowInInspector]
		private Dictionary<int, FightCircleSubject> engagedSlots;
		public bool gizmo_draw = false;


		private Color GIZMO_RADIUS_MIN = new Color(1.000f, 0.688f, 0.000f, 1.000f);
		private Color GIZMO_RADIUS_MAX = new Color(1.000f, 0.000f, 0.089f, 1.000f);

		private void Awake(){
			engagedSlots = new Dictionary<int, FightCircleSubject>();
		}


		public void Draw(){
			var center = transform.position;
			DebugDrawing.DrawCircle(center, radius, GIZMO_RADIUS_MIN);
			DebugDrawing.DrawCircle(center, maxRadius, GIZMO_RADIUS_MAX);
		}

		private void OnDrawGizmos(){
			if (gizmo_draw){
				Draw();
			}
		}

		public override CircleType GetCircleType(){
			return CircleType.Range;
		}

		public override bool IsContains(FightCircleSubject enemy){
			return engagedSlots.ContainsKey(enemy.GetInstanceID());
		}

		public override bool Register(FightCircleSubject enemy){
			int instance = enemy.GetInstanceID();
			if (engagedSlots.ContainsKey(instance))
				return false;
			if (engagedSlots.Count >= maximumSlots)
				return false;
			float availWeight = GetAvailableSlotsWeight();
			float enemyWeight = enemy.GetSlotWeight();
			if (availWeight < enemyWeight)
				return false;
			engagedSlots.Add(instance, enemy);
			SetAvailableSlotsWeight(availWeight - enemyWeight);
			return true;
		}

		public override bool Unregister(FightCircleSubject enemy){
			bool isContains = IsContains(enemy);
			int instance = enemy.GetInstanceID();
			if (isContains){
				SetAvailableSlotsWeight(slotsWeight + enemy.GetSlotWeight());
				engagedSlots.Remove(instance);
			}
			return isContains;
		}

		public override void ClearAll(){
			engagedSlots.Clear();
		}
	}
}
