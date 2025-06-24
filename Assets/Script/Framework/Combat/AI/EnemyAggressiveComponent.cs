using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 战斗系统进攻性得分：
	/// 	1. 敌人能否攻击（确保敌人不在受击或处于其他不可攻击状态下）
	/// 	2. 明确进攻性的优先级（基于敌人类型设计的数值体系）
	/// 	3. 玩家是否锁定正在这个敌人
	/// 	4. 动作等级，基于三点做判断，a.是否处于屏幕中 b.位置与相机的夹角 c.与玩家的距离
	/// 通过调整各项得分数值控制敌人何时可获取进攻性，进入近战圈
	/// 详细概念参考 战神4 GDC：www.bilibili.com/video/BV1oZ4y1q7ud
	///
	/// todo 非玩家目标时，那些根据摄像机计算的得分可能表现不佳。
	/// </summary>
	public class EnemyAggressiveComponent : FrameworkComponent{
		[LabelText("可攻击分权重")] public float attackableScoreWeight = 1;
		[LabelText("可见分权重")] public float visibleScoreWeight = 1;
		[LabelText("相机夹角分权重")] public float angleScore = 1;
		[LabelText("距离分权重")] public float distanceScoreWeight = 1;


		private float MaxDistance = 10;
		private float MaxAngle = 90;
		[Header("Debug")]
		public List<AggressiveScoreInfo> aggressiveScoreInfos;

		private void Start(){
			aggressiveScoreInfos = new List<AggressiveScoreInfo>();
		}

		/// <summary>
		/// 照敌人列表计算得分排序
		/// </summary>
		/// <param name="enemies"></param>
		/// <param name="targetCharacter"></param>
		public List<EnemySortData> SortEnemiesByAggressiveScore(List<FightCircleSubject> enemies,
			BaseCharacter targetCharacter){
			aggressiveScoreInfos.Clear();
			List<EnemySortData> sortedList = new List<EnemySortData>(enemies.Count);
			for (int i = 0; i < enemies.Count; i++){
				FightCircleSubject enemy = enemies[i];
				AggressiveScoreInfo aggressiveScoreInfo = GetAggressiveScore(enemy, targetCharacter);
				float score = aggressiveScoreInfo.GetTotalScore();
				sortedList.Add(new EnemySortData(score, enemy));
				aggressiveScoreInfos.Add(aggressiveScoreInfo);
			}
			aggressiveScoreInfos.Sort((info, scoreInfo) => scoreInfo.GetTotalScore().CompareTo(info.GetTotalScore()));
			// sortedList.Sort((data, sortData) => sortData.score.CompareTo(data.score));
			sortedList.Sort((data, sortData) => data.CompareTo(sortData));
			return sortedList;
		}

		/// <summary>
		/// 进攻性得分计算
		/// </summary>
		/// <param name="aiController"></param>
		/// <param name="target"></param>
		private AggressiveScoreInfo GetAggressiveScore(FightCircleSubject aiController, BaseCharacter target){
			AggressiveScoreInfo aggressiveScoreInfo = new AggressiveScoreInfo();
			//1.可攻击计算
			if (aiController.CanAttack())
				aggressiveScoreInfo.attackableScore = attackableScoreWeight;
			//2.AI类型优先级计算
			aggressiveScoreInfo.fightPriority = aiController.GetFightPriority();
			//3. todo 玩家是否锁定这个敌人

			//4.动作等级，基于三点做判断，
			Vector3 aiPosition = aiController.GetPosition();
			// a.是否处于屏幕中
			if (ObjectVisible(aiPosition)){
				aggressiveScoreInfo.visibleScore = visibleScoreWeight;
			}
			//b.位置与相机的夹角
			float angle = ObjectCameraAngle(aiPosition);
			aggressiveScoreInfo.angleScore = Mathf.Lerp(angleScore, 0, angle / MaxAngle); //夹角越大得分越低，到达90°将无得分
			//c.与目标角色的距离
			// float actionRankScore = 0;
			float distance = Vector3.Distance(target.GetPosition(), aiPosition);
			float disScore = Mathf.Lerp(distanceScoreWeight, 0, distance / MaxDistance); //距离越近得分越高，到达10米时将无得分
			aggressiveScoreInfo.distanceScore = disScore;
			aggressiveScoreInfo.aiGO = aiController.transform.gameObject;
			return aggressiveScoreInfo;
		}


		/// <summary>
		/// </summary>
		public bool ObjectVisible(Vector3 pos){
			var playerCamera = GameEntry.Main.cameraController.TargetCamera;
			//转化为视角坐标
			Vector3 viewPos = playerCamera.WorldToViewportPoint(pos);
			// z<0代表在相机背后
			if (viewPos.z < 0) return false;
			if (viewPos.z > playerCamera.farClipPlane)
				return false;
			// x,y取值在 0~1之外时代表在视角范围外；
			if (viewPos.x < 0 || viewPos.y < 0 || viewPos.x > 1 || viewPos.y > 1) return false;
			return true;
		}

		public float ObjectCameraAngle(Vector3 pos){
			var cameraTran = GameEntry.Main.cameraController.transform;
			float angle = Vector3.Angle(cameraTran.forward, pos - cameraTran.position);
			return angle;
		}


	}

	public struct EnemySortData{
		public FightCircleSubject enemy;
		public float score;

		// public int index;
		public EnemySortData(float score, FightCircleSubject enemy){
			this.score = score;
			this.enemy = enemy;
		}

		public int CompareTo(EnemySortData other){
			if (this.score != other.score){
				return other.score.CompareTo(this.score);
			} else return 0;
		}
	}


	[Serializable]
	public struct AggressiveScoreInfo{
		[LabelText("总分"), SerializeField, ReadOnly]
		private float totalScore;
		[LabelText("可攻击分"), ReadOnly] public float attackableScore;
		[LabelText("优先级分"), ReadOnly] public float fightPriority;
		[LabelText("可见分"), ReadOnly] public float visibleScore;
		[LabelText("相机夹角分"), ReadOnly] public float angleScore;
		[LabelText("玩家距离分"), ReadOnly] public float distanceScore;
		[LabelText("GO"), ReadOnly] public GameObject aiGO;

		public float GetTotalScore(){
			totalScore = attackableScore + fightPriority + visibleScore + angleScore + distanceScore;
			return totalScore;
		}
	}
}
