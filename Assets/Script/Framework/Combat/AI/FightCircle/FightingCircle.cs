using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 战斗圈实现，管理敌人站位、进攻
	/// 详细概念：www.gameaipro.com/GameAIPro/GameAIPro_Chapter28_Beyond_the_Kung-Fu_Circle_A_Flexible_System_for_Managing_NPC_Attacks.pdf
	/// </summary>
	[RequireComponent(typeof(MeleeCircle))]
	[RequireComponent(typeof(ApproachCircle))]
	[RequireComponent(typeof(RangeCircle))]
	[RequireComponent(typeof(WaitingCircle))]
	public class FightingCircle : CharacterComponent{
		[ShowInInspector]
		private Dictionary<int, FightCircleData> enemies;
		[ShowInInspector]
		private Dictionary<CircleType, BaseCircle> circles;

		[SerializeField]
		private float attackTokenMax = 3;
		[SerializeField]
		private float randomFactor = 1;

		public bool debug;
		private float attackToken;
		private int frameCount = 0;
		private const int StealInterval = 5;
		private BaseCharacter ownerCharacter;


		protected override void OnInit(BaseCharacter character){
			base.OnInit(character);
			circles = new Dictionary<CircleType, BaseCircle>();
			var dataCircles = GetComponents<BaseCircle>();
			for (int i = 0; i < dataCircles.Length; i++){
				circles.Add(dataCircles[i].GetCircleType(), dataCircles[i]);
			}
			enemies = new Dictionary<int, FightCircleData>();
			attackToken = attackTokenMax;
			ownerCharacter = owner;
		}


		/// <summary>
		/// </summary>
		private void Update(){
			frameCount++;
			if (frameCount > StealInterval){
				frameCount = 0;
				ReassignBehaviour();
			}
		}


		/// <summary>
		/// 根据当前世界状态重新分配Slot
		/// Attacking list = all attacking creatures
		/// For each creature in Attacking list
		/// Find closest slot for creature
		/// 	If closest slot is locked, continue
		/// Assign closest slot to creature
		/// 	Remove creature from Attacking list
		/// If closest slot was already assigned,
		/// 	Remove assignment from that creature
		/// </summary>
		private void ReassignBehaviour(){
			var waitingCircle = circles[CircleType.Wait];
			var meleeCircle = GetMeleeCircle();
			var rangeCircle = GetRangeCircle();
			var approachCircle = GetApproachCircle();
			var enemiesData = enemies.Values
				.Where(e => e.fightCircleSubject != null)
				.ToList();
			var nonFreeSubjects = new HashSet<int>();
			var nonFreeSlots = new HashSet<int>();

			// Step 1: 清空原有分配，全部注册到等待圈
			foreach (var enemy in enemiesData){
				circles[enemy.type].Unregister(enemy.fightCircleSubject);
				enemy.type = CircleType.Wait;
				waitingCircle.Register(enemy.fightCircleSubject);
			}

			// Step 2: 分配近战敌人到攻击圈
			AssignMeleeEnemiesToMeleeCircle(enemiesData, meleeCircle, nonFreeSubjects, nonFreeSlots);

			// Step 3: 分配远程敌人到远程圈
			AssignRangedEnemiesToRangeCircle(enemiesData, rangeCircle, nonFreeSubjects);

			// Step 4: 剩余敌人分配到接近圈
			AssignRemainingEnemiesToApproachCircle(enemiesData, approachCircle, nonFreeSubjects);
		}

		/// <summary>
		/// 分配近战圈
		/// </summary>
		private void AssignMeleeEnemiesToMeleeCircle(List<FightCircleData> enemiesData, MeleeCircle meleeCircle,
			HashSet<int> nonFreeSubjects, HashSet<int> nonFreeSlots){
			meleeCircle.UpdateNodeValidity();
			///可以进入近战的类型
			var meleeSubjects = enemiesData
				.Where(e => e.fightCircleSubject.GetCombatStyle() == CombatStyle.Melee || e.fightCircleSubject.GetCombatStyle() == CombatStyle.Both)
				.Select(e => e.fightCircleSubject)
				.ToList();
			var sorted = GameEntry.Aggressive.SortEnemiesByAggressiveScore(meleeSubjects, ownerCharacter);
			var freePositions = meleeCircle.GetGlobalPositions();
			for (int i = sorted.Count - 1; i >= 0; i--){
				var subject = sorted[i].enemy;
				var id = subject.GetInstanceID();
				if (enemies[id].isLock && meleeCircle.IsContains(subject)){//锁定状态，并且已经在处于该圈内将忽略
					int slot = meleeCircle.GetSlot(subject);
					nonFreeSubjects.Add(id);
					nonFreeSlots.Add(slot);
					meleeCircle.SetInstanceSlots(subject, slot);
					sorted.RemoveAt(i);
				}
			}
			foreach (var sortData in sorted){
				var subject = sortData.enemy;
				//角色与slot点基于距离匹配
				if (meleeCircle.FindNearestSlotToEnemy(subject, freePositions, nonFreeSlots.ToList(), out int slot)){
					if (Move(subject, CircleType.Melee)){
						nonFreeSlots.Add(slot);
						nonFreeSubjects.Add(subject.GetInstanceID());
						meleeCircle.SetInstanceSlots(subject, slot);
						subject.OnCircleUpdate(CircleType.Melee);
					}
				}
			}
		}

		/// <summary>
		/// 分配远程圈
		/// </summary>
		private void AssignRangedEnemiesToRangeCircle(
			List<FightCircleData> enemiesData, RangeCircle rangeCircle, HashSet<int> nonFreeSubjects){
			float minRadius = rangeCircle.radius;
			float maxRadius = rangeCircle.maxRadius;
			float avgRadius = (minRadius + maxRadius) * 0.5f;
			int maxSlots = rangeCircle.maximumSlots;
			for (int i = 0; i < maxSlots; i++){
				float minDist = float.MaxValue;
				FightCircleSubject chosen = null;
				foreach (var e in enemiesData){
					var s = e.fightCircleSubject;
					if (s.GetCombatStyle() == CombatStyle.Melee) continue;
					if (nonFreeSubjects.Contains(s.GetInstanceID())) continue;
					float distance = Vector3.Distance(s.GetPosition(), transform.position);
					float score = Mathf.Abs(distance - avgRadius) +
						UnityEngine.Random.value * randomFactor; //增加随机因子来实现原地不动时不会总是相同的敌人攻击玩家
					if (score < minDist){
						minDist = score;
						chosen = s;
					}
				}
				if (chosen != null && Move(chosen, CircleType.Range)){
					nonFreeSubjects.Add(chosen.GetInstanceID());
					chosen.OnCircleUpdate(CircleType.Range);
				}
			}
		}

		/// <summary>
		/// 分配接近圈
		/// </summary>
		private void AssignRemainingEnemiesToApproachCircle(
			List<FightCircleData> enemiesData, ApproachCircle approachCircle, HashSet<int> nonFreeSubjects){
			approachCircle.UpdateNodeValidity();
			var globalPositions = approachCircle.GetGlobalPosition();
			var usedSlots = new HashSet<int>();
			foreach (var e in enemiesData){
				if (e.isLock) continue;
				var s = e.fightCircleSubject;
				if (nonFreeSubjects.Contains(s.GetInstanceID())) continue;
				if (approachCircle.FindNearestSlotToEnemy(s, globalPositions, usedSlots.ToList(), out int slot)){
					if (Move(s, CircleType.Approach)){
						usedSlots.Add(slot);
						nonFreeSubjects.Add(s.GetInstanceID());
						approachCircle.SetInstanceSlots(s, slot);//查询当前最近接近圈点位后并设置对应索引
						s.OnCircleUpdate(CircleType.Approach);
					}
				}
			}
		}

		/// <summary>
		/// 在战斗圈内注册
		/// </summary>
		/// <param name="enemy"></param>
		/// <param name="type"></param>
		public bool Register(FightCircleSubject enemy, out CircleType type){
			type = CircleType.Melee;
			if (IsContains(enemy)){
				type = enemies[enemy.GetInstanceID()].type;
				return true;
			}
			bool isRegistered = false;
			switch (enemy.GetCombatStyle()){
				case CombatStyle.Melee:
					type = CircleType.Melee;
					isRegistered = Register(enemy, type);
					break;
				case CombatStyle.Range:
					type = CircleType.Range;
					isRegistered = Register(enemy, type);
					break;
				case CombatStyle.Both:
					type = CircleType.Melee;
					isRegistered = Register(enemy, type);
					if (!isRegistered){
						type = CircleType.Range;
						isRegistered = Register(enemy, type);
					}
					break;
			}
			if (!isRegistered){
				type = CircleType.Approach;
				isRegistered = Register(enemy, type);
			}
			return isRegistered;
		}


		/// <summary>
		/// 在战斗圈内的目标圈注册，如已注册移动至目标圈内
		/// </summary>
		/// <param name="enemy"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private bool Register(FightCircleSubject enemy, CircleType type){
			if (IsContains(enemy)){
				return Move(enemy, type);
			}
			var circle = circles[type];
			bool isRegistered = circle.Register(enemy);
			if (isRegistered){
				enemies.Add(enemy.GetInstanceID()
					, new FightCircleData(enemy, type));
				enemy.SetCircleType(type);
			}
			return isRegistered;
		}


		private bool Move(FightCircleSubject enemy, CircleType to){
			int instance = enemy.GetInstanceID();
			var data = enemies[instance];
			if (to.Equals(data.type))
				return true;
			var fromCircle = circles[data.type];
			var toCircle = circles[to];
			bool isSwapped = toCircle.Register(enemy);
			if (isSwapped){
				fromCircle.Unregister(enemy);
				data.type = to;
				enemy.SetCircleType(to);
			}
			return isSwapped;
		}

		public bool Unregister(FightCircleSubject enemy){
			int instance = enemy.GetInstanceID();
			if (!enemies.ContainsKey(instance))
				return false;
			var type = enemies[instance].type;
			var circle = circles[type];
			bool isUnregistered = circle.Unregister(enemy);
			if (isUnregistered){
				enemies.Remove(instance);
			}
			return isUnregistered;
		}

		/// <summary>
		/// 是否在战斗圈内有注册
		/// </summary>
		/// <param name="enemy"></param>
		/// <returns></returns>
		public bool IsContains(FightCircleSubject enemy){
			return enemies.ContainsKey(enemy.GetInstanceID());
		}

		public MeleeCircle GetMeleeCircle(){
			return circles[CircleType.Melee] as MeleeCircle;
		}

		/// <summary>
		/// 取近战圈的期望坐标
		/// </summary>
		/// <param name="enemy"></param>
		/// <returns></returns>
		public Vector3 GetSlotPositionFromMeleeCircle(FightCircleSubject enemy){
			var melee = GetMeleeCircle();
			int instance = enemy.GetInstanceID();
			// var type = melee.GetGlobalPosition(enemy);
			return melee.GetGlobalPositionByInstance(instance);
		}

		/// <summary>
		/// 取接近圈的期望坐标
		/// </summary>
		/// <param name="enemy"></param>
		/// <returns></returns>
		public Vector3[] GetSlotPositionFromApproachCircle(FightCircleSubject enemy){
			var approach = GetApproachCircle();
			int instance = enemy.GetInstanceID();
			var type = approach.GetGlobalPosition(enemy);
			return approach.GetGlobalPositionByInstance(instance);
		}

		/// <summary>
		/// 取远程圈的期望坐标
		/// </summary>
		/// <param name="enemy"></param>
		/// <param name="center"></param>
		/// <returns></returns>
		public Vector3 GetSlotPositionFromRangeCircle(FightCircleSubject enemy,Vector3 center){
			var rangeCircle = GetRangeCircle();
			Vector3 directionToTarget = (enemy.GetPosition() - ownerCharacter.GetPosition()).normalized;
			var targetPoint = directionToTarget.normalized * (rangeCircle.radius);
			return targetPoint + center;
		}

		public ApproachCircle GetApproachCircle(){
			return circles[CircleType.Approach] as ApproachCircle;
		}

		public RangeCircle GetRangeCircle(){
			return circles[CircleType.Range] as RangeCircle;
		}

		public BaseCircle GetCircle(CircleType circleTypeType){
			return circles[circleTypeType];
		}


		/// <summary>
		/// 是否允许注册动作
		/// </summary>
		public bool CanRegisterAction(FightCircleSubject enemy, float count){
			int instance = enemy.GetInstanceID();
			if (!enemies.ContainsKey(instance)){
				if (debug)
					Debug.LogError("战斗圈内不存在该角色，无法申请攻击Token", enemy.transform.gameObject);
				return false;
			}
			if (attackToken < count){
				if (debug)
					Debug.Log($"不允许申请 当前token{attackToken}  申请消耗{count}", enemy.transform.gameObject);
				return false;
			}
			if (debug)
				Debug.Log($"允许申请 当前token{attackToken}  申请消耗{count}", enemy.transform.gameObject);
			return true;
		}

		/// <summary>
		/// 注册动作来占用权重并锁定攻击圈状态，如某个敌人释放高威力动作时限制其他敌人进入攻击圈
		/// </summary>
		public bool RegisterActionToken(FightCircleSubject enemy, float weight){
			int instance = enemy.GetInstanceID();
			if (!enemies.ContainsKey(instance))
				return false;
			if (attackToken < weight)
				return false;
			enemies[instance].isLock = true;
			attackToken -= weight;
			if (debug)
				Debug.Log($"申请token{weight} 剩余{attackToken}", enemy.transform.gameObject);
			return true;
		}

		/// <summary>
		/// 取消注册动作，释放动作过程中将锁定所在圈
		/// </summary>
		public bool UnregisterActionToken(FightCircleSubject enemy, float count){
			int instance = enemy.GetInstanceID();
			if (!enemies.ContainsKey(instance))
				return false;
			//如果不在锁定状态代表已释放
			if (!enemies[instance].isLock) return false;
			enemies[instance].isLock = false;
			attackToken = Mathf.Clamp(attackToken + count, 0f, attackTokenMax);
			if (debug)
				Debug.Log($"返还token{count} 剩余{attackToken}", enemy.transform.gameObject);
			return true;
		}

		public void ClearAll(){
			foreach (var enemyPair in enemies){
				if (enemyPair.Value.isLock) continue;
				circles[enemyPair.Value.type].Unregister(enemyPair.Value.fightCircleSubject);
			}
			enemies.Clear();
			foreach (var circle in circles){
				circle.Value.ClearAll();
			}
			attackToken = attackTokenMax;
		}
	}

	public class FightCircleData{
		public FightCircleSubject fightCircleSubject;
		public CircleType type;
		public bool isLock;

		public FightCircleData(FightCircleSubject fightCircleSubject, CircleType type){
			this.fightCircleSubject = fightCircleSubject;
			this.type = type;
		}
	}

	public enum NodeState{
		Valid,
		Used,
		InValid,
	}

	public struct FightingNode{
		public int index;
		public NodeState state;
	}
}
