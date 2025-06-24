using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Combat.Runtime{
	public class ApproachCircle : BaseCircle{
		private List<Vector3[]> localPositions;

		private Color GIZMO_RADIUS_START = Color.blue;
		private Color GIZMO_RADIUS_END = new Color(0.5f, 0.2f, 1, 1);


		public float offsetBetweenPointDegrees = 5f;
		public Vector3 offsetBetweenPointsAppend = Vector3.zero;
		[Title("Debug")]
		[ShowInInspector]
		private Dictionary<int, EngagedInfo> engagedSlots;
		public bool gizmo_draw = false;
		public float GIZMO_RADIUS_POINT = 0.5f;
		public const float MaxApproachDistance = 99f;


		public void Draw(){
			var points = GetGlobalPosition();
			for (int i = 0; i < points.Count; i++){
				var x = points[i];
				var color = GIZMO_RADIUS_START;
				if (fightingNodes != null){
					if (fightingNodes[i].state == NodeState.InValid)
						color = Color.gray;
					else if (fightingNodes[i].state == NodeState.Used)
						color = Color.red;
				}
				DebugDrawing.DrawCircle(x[0], GIZMO_RADIUS_POINT, color);
				// DebugDrawing.DrawCircle(x[1], GIZMO_RADIUS_POINT, GIZMO_RADIUS_END);
			}
		}

		private void OnDrawGizmos(){
			if (gizmo_draw){
				Draw();
			}
		}
		
		private void OnValidate(){
			if (!Application.isPlaying)
				Awake();
			else{
				localPositions = new List<Vector3[]>();
				var arc = degrees / maximumSlots;
				for (int i = 0; i < maximumSlots; i++){
					Vector3 positionOffset = ComputePositionOffset((arc * (i + 1)), radius,
						offsetBetweenPointsAppend);
					Vector3[] posPair = new Vector3[]
						{ComputePosition(arc * i, radius), ComputePosition(arc * i, radius)};
					localPositions.Add(posPair);
				}
			}
		}


		private void Awake(){
			localPositions = new List<Vector3[]>();
			var arc = degrees / maximumSlots;
			for (int i = 0; i < maximumSlots; i++){
				Vector3 positionOffset = ComputePositionOffset((arc * (i + 1)), radius,
					offsetBetweenPointsAppend);
				Vector3[] posPair = new Vector3[]{ComputePosition(arc * i, radius), positionOffset};
				localPositions.Add(posPair);
				// (new Pair<Vector3, Vector3>(ComputePosition(arc * i, radius)
				// , ComputePosition(arc * i, radius)));
			}
			engagedSlots = new Dictionary<int, EngagedInfo>();
		}


		public override void UpdateNodeValidity(){
			var globalPos = GetGlobalPosition();
			for (int i = 0; i < globalPos.Count; i++){
				var start = globalPos[i][0];
				var end = transform.position;
				start.y = end.y += 1f;
				if (Physics.Linecast(start, end, LayerManager.Default, QueryTriggerInteraction.Ignore)){
					fightingNodes[i].state = NodeState.InValid;
				} else{
					fightingNodes[i].state = NodeState.Valid;
				}
			}
		}




		protected Vector3 ComputePositionOffset(float degrees, float radius, Vector3 offset){
			return new Vector3
			(Mathf.Cos(Mathf.Deg2Rad * degrees) * radius + offset.x
				, 0f
				, Mathf.Sin(Mathf.Deg2Rad * degrees) * radius + offset.z);
		}


		public override CircleType GetCircleType(){
			return CircleType.Approach;
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
			engagedSlots.Add(instance
				, new EngagedInfo(){subject = enemy, slotIndex = 0});
			SetAvailableSlotsWeight(availWeight - enemyWeight);
			// FindNearestSlotToEnemies();
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

		internal void SetInstanceSlots(FightCircleSubject enemy, int slot){
			if (engagedSlots.ContainsKey(enemy.GetInstanceID())){
				EngagedInfo engagedSlot = engagedSlots[enemy.GetInstanceID()];
				engagedSlot.slotIndex = slot;
				engagedSlots[enemy.GetInstanceID()] = engagedSlot;
				fightingNodes[slot].state = NodeState.Used;
			}
		}


		public bool FindNearestSlotToEnemy(FightCircleSubject enemy
			, List<Vector3[]> globalPositions, List<int> nonFreeSlots, out int index){
			var pos = enemy.GetPosition();
			int imin = -1;
			float minDist = MaxApproachDistance;
			for (int i = 0; i < globalPositions.Count; i++){
				if (fightingNodes[i].state == NodeState.InValid) continue;
				if (nonFreeSlots.Contains(i)) continue;
				float distance = Vector3.Distance(pos, globalPositions[i][0]); //取首个点做匹配计算，也许可以考虑用中心点做计算
				if (!(distance < minDist)) continue;
				minDist = distance;
				imin = i;
			}
			index = imin;
			return index != -1;
		}

		/// <summary>
		/// 接近圈目标返回两个点，允许AI在两个点之间踱步
		/// </summary>
		public Vector3[] GetGlobalPosition(int slot){
			return new Vector3[2]{
				localPositions[slot][0] + transform.position, localPositions[slot][1] + transform.position
			};
		}

		public Vector3[] GetGlobalPosition(FightCircleSubject enemy){
			int slot = engagedSlots[enemy.GetInstanceID()].slotIndex;
			return GetGlobalPosition(slot);
		}

		public Vector3[] GetGlobalPositionByInstance(int instance){
			int slot = engagedSlots[instance].slotIndex;
			return GetGlobalPosition(slot);
		}

		public List<Vector3[]> GetGlobalPosition(){
			var center = transform.position;
			var ret = new List<Vector3[]>();
			for (int i = 0; i < localPositions.Count; i++){
				ret.Add
				(new Vector3[]{
					localPositions[i][0] + center, localPositions[i][1] + center
				});
			}
			return ret;
		}

		public override void ClearAll(){
			engagedSlots.Clear();
		}

		private struct EngagedInfo{
			public FightCircleSubject subject;
			public int slotIndex;
		}
	}
}
