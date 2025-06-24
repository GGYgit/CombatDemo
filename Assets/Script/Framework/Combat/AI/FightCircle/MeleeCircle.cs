using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 近战圈，以角色为中心点的若干个点
	/// </summary>
	[Serializable]
	public class MeleeCircle : BaseCircle{
		public const float MaxAttackDistance = 99f;
		private struct EngagedInfo{
			public FightCircleSubject subject;
			public int slotIndex;
		}
		[SerializeField]
		private Vector3[] localPositions;

		public float arc = 60;
		public float possibleTargetWeight = 1f;

		[Header("Debug")]
		public bool gizmo_draw = false;
		[ShowInInspector]
		private Dictionary<int, EngagedInfo> engagedSlots; //instance Id , Subject
		private float GIZMO_RADIUS_POINT = 0.5f;
		private Color GIZMO_COLOR_POINT = Color.green;

		private void Awake(){
			localPositions = new Vector3[maximumSlots];
			// var arc = degrees / maximumSlots;
			for (int i = 0; i < maximumSlots; i++){
				var angle = arc * (i + 1);
				localPositions[i] = ComputePosition(angle, radius);
			}
			engagedSlots = new Dictionary<int, EngagedInfo>();
		}

		private void OnValidate(){
			if (!Application.isPlaying)
				Awake();
		}

		private void OnDrawGizmos(){
			if (gizmo_draw){
				DrawDebugGizmos();
			}
		}

		private void DrawDebugGizmos(){
			for (int i = 0; i < maximumSlots; i++){
				var color = GIZMO_COLOR_POINT;
				Vector3 pos = GetGlobalPosition(i);
				if (fightingNodes != null){
					if (fightingNodes[i].state == NodeState.InValid)
						color = Color.gray;
					else if (fightingNodes[i].state == NodeState.Used){
						color = Color.red;
					}
				}
				DebugDrawing.DrawCircle(pos, GIZMO_RADIUS_POINT, color);
				// DebugDrawing.DrawCircle
				// (GetGlobalPosition(i)
				// , GIZMO_RADIUS_POINT, color);
			}
		}

		public override void UpdateNodeValidity(){
			var globalPos = GetGlobalPositions();
			for (int i = 0; i < globalPos.Count; i++){
				var start = globalPos[i];
				var end = transform.position;
				start.y = end.y += 1f;
				if (Physics.Linecast(start, end, LayerManager.Default, QueryTriggerInteraction.Ignore)){
					fightingNodes[i].state = NodeState.InValid;
				} else{
					fightingNodes[i].state = NodeState.Valid;
				}
			}
		}

		public int GetSlot(FightCircleSubject enemy){
			int slot = engagedSlots[enemy.GetInstanceID()].slotIndex;
			return slot;
		}

		public Vector3 GetGlobalPosition(int slot){
			return localPositions[slot] + transform.position;
		}

		public Vector3 GetGlobalPosition(FightCircleSubject enemy){
			int slot = engagedSlots[enemy.GetInstanceID()].slotIndex;
			return GetGlobalPosition(slot);
		}

		public Vector3 GetGlobalPositionByInstance(int instance){
			int slot = engagedSlots[instance].slotIndex;
			return GetGlobalPosition(slot);
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
			engagedSlots.Add(instance
				, new EngagedInfo(){subject = enemy, slotIndex = 0});
			SetAvailableSlotsWeight(availWeight - enemyWeight);
			return true;
		}

		public override bool IsContains(FightCircleSubject enemy){
			return engagedSlots.ContainsKey(enemy.GetInstanceID());
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

		public void Update(){
			// FindNearestSlotToEnemies();
		}


		/*
		public void FindNearestSlotToEnemies(){
			var center = transform.position;
			var freePositions = GetGlobalPositions();
			var nonFreeSlots = new List<int>();
			foreach (var val in engagedSlots.Values){
				var enemy = val.First;
				int slot = FindNearestSlotToEnemy(enemy, freePositions, nonFreeSlots);
				nonFreeSlots.Add(slot);
				engagedSlots[enemy.GetInstanceID()].Second = slot;
			}
		}*/

		internal void SetInstanceSlots(FightCircleSubject enemy, int slot){
			if (engagedSlots.ContainsKey(enemy.GetInstanceID())){
				var engagedSlot = engagedSlots[enemy.GetInstanceID()];
				engagedSlot.slotIndex = slot;
				engagedSlots[enemy.GetInstanceID()] = engagedSlot;
				fightingNodes[slot].state = NodeState.Used;
			}
		}

		/// <summary>
		/// 获取距离敌人最接近的槽位
		/// </summary>
		public bool FindNearestSlotToEnemy(FightCircleSubject enemy
			, List<Vector3> globalPositions, List<int> nonFreeSlots, out int index){
			var pos = enemy.GetPosition();
			int imin = -1;
			float minDist = MaxAttackDistance;
			for (int i = 0; i < globalPositions.Count; i++){
				//槽位是否有效，槽位是否已被占用
				if (fightingNodes[i].state != NodeState.Valid) continue;
				if (nonFreeSlots.Contains(i)) continue;
				float distance = Vector3.Distance(pos, globalPositions[i]);
				if (!(distance < minDist)) continue;
				minDist = distance;
				imin = i;
			}
			index = imin;
			return index != -1;
		}

		/// <summary>
		///  按每个敌人与距离最近攻击圈的距离排序
		/// </summary>
		/// <param name="enemies"></param>
		/// <returns></returns>
		// public List<EnemySortData> SortNearestEnemy(List<KungFuCircleSubject> enemies){
		// 	var globalPositions = GetGlobalPositions();
		// 	List<EnemySortData> enemySortedList = new List<EnemySortData>();
		// 	for (int i = 0; i < enemies.Count; i++){
		// 		float minDist = 0;
		// 		var pos = enemies[i].GetPosition();
		// 		int index = 0;
		// 		for (int j = 0; j < globalPositions.Count; j++){
		// 			float distance = Vector3.Distance(pos, globalPositions[i]);
		// 			if (!(distance < minDist)) continue;
		// 			minDist = distance;
		// 			index = j;
		// 		}
		// 		enemySortedList.Add(new EnemySortData(){
		// 			enemy = enemies[i],
		// 			index = index,
		// 			minDist = minDist
		// 		});
		// 	}
		// 	enemySortedList.Sort();
		// 	return enemySortedList;
		// }
		public List<Vector3> GetGlobalPositions(){
			var center = transform.position;
			List<Vector3> ret = new List<Vector3>();
			for (int i = 0; i < localPositions.Length; i++){
				ret.Add(localPositions[i] + center);
			}
			return ret;
		}

		private List<Vector3> GetGlobalFreePositions(){
			var center = transform.position;
			List<Vector3> ret = new List<Vector3>();
			var engaged = engagedSlots.Values.ToList().ConvertAll(x => x.slotIndex);
			for (int i = 0; i < localPositions.Length; i++){
				if (!engaged.Contains(i)){
					ret.Add(localPositions[i] + center);
				}
			}
			return ret;
		}


		public override CircleType GetCircleType(){
			return CircleType.Melee;
		}

		public override void ClearAll(){
			engagedSlots.Clear();
		}
	}
}
