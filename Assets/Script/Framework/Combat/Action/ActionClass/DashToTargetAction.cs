using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 冲刺到目标处动作，优先锁定远程敌人，目前仅用于玩家跳跃下冲
	/// </summary>
	public class DashToTargetAction : BaseAction{
		public AnimationConfig dashAnimation;
		[FormerlySerializedAs("animationConfig")]
		public AnimationConfig attackConfig;
		public float range = 30;
		public float lockHeight = 0.2f;
		public float dashMaxTime = 0.5f;
		public AnimationCurve dashCurve = new AnimationCurve(
			new Keyframe(0f, 0f, 0f, 2f), // 缓慢起步
			new Keyframe(0.3f, 0.15f),
			new Keyframe(0.5f, 0.85f),
			new Keyframe(0.8f, 0.97f),
			new Keyframe(1f, 1f, 0f, 0f) // 平滑结束
		);

		public float reachThreshold = 1.0f; // 到达判定距离
		private float rotateSpeed = 480;
		private Vector3 dashStart;
		private Vector3 dashEnd;
		private float currentTime;
		private float lastEvaluatedT = 0f;


		private DashState dashState = DashState.Dash;
		private BaseCharacter targetChar;

		protected override void OnActivated(object userdata = null){
			var targetComp = OwnerChar.GetCharComponent<TargetManagerComponent>();
			targetChar = null;
			if (targetComp){
				targetChar = targetComp.GetCurrentTarget();
				if (targetChar == null){
					List<BaseCharacter> possibleTargets = OwnerChar.GetPossibleTargets(range, 0.5f, 20f, true);
					foreach (var possibleTarget in possibleTargets){
						if (possibleTarget is EnemyCharacter enemyCharacter &&
						    enemyCharacter.enemyType == EnemyType.Ranged){
							targetChar = possibleTarget;
							break;
						}
					}
					if (targetChar == null && possibleTargets.Count > 0){
						targetChar = possibleTargets[0];
					}
				}
			}
			if (targetChar == null){
				Debug.LogError("无有效锁定目标，无法释放");
				ExitAction();
				return;
			}
			dashStart = OwnerChar.GetPosition();
			dashState = DashState.Dash;
			dashEnd = targetChar.GetPosition();
			// 冲刺目标点为敌人当前位置上方（略抬高）
			dashEnd.y = targetChar.GetPosition().y + lockHeight;
			currentTime = 0f;
			lastEvaluatedT = 0f;
			OwnerChar.StatusComp.AddStatus(CharacterStatusType.Invincible, dashMaxTime);
			OwnerChar.MovementComp.DisableGravity();
			ExecuteAction(dashAnimation);
			OwnerChar.DisableCollider();
		}

		protected override void OnTick(float deltaTime){
			if (dashState != DashState.Dash) return;
			currentTime += deltaTime;
			float t = Mathf.Clamp01(currentTime / dashMaxTime);
			float curvedT = dashCurve.Evaluate(t);
			float deltaT = curvedT - lastEvaluatedT;
			lastEvaluatedT = curvedT;
			Vector3 total = dashEnd - dashStart;
			Vector3 deltaMove = total * deltaT;
			OwnerChar.Move(deltaMove);
			OwnerChar.MovementComp.RotateToPoint(dashEnd, rotateSpeed);
			if (t >= 1f || targetChar.CalculateDistanceBetweenCharactersExtents(OwnerChar) < reachThreshold){
				ExecuteAction(attackConfig);
				OwnerChar.EnableCollider();
				OwnerChar.MovementComp.EnableGravity();
				dashState = DashState.Attack;
				SnapToGround();
			}
		}


		private void SnapToGround(){
			if (Physics.Raycast(OwnerChar.GetPosition(), Vector3.down, out RaycastHit hit, 2f, LayerManager.Default,
				    QueryTriggerInteraction.Ignore)){
				OwnerChar.MovementComp.Move(new Vector3(0, -hit.distance, 0));
			}
		}


		protected override void OnDeactivated(){
			base.OnDeactivated();
			OwnerChar.StatusComp.RemoveStatus(CharacterStatusType.Invincible);
			OwnerChar.EnableCollider();
			OwnerChar.MovementComp.EnableGravity();
		}

		/// <summary>
		/// 覆盖默认实现的动画播放完成就退出动作
		/// </summary>
		protected override void OnAnimEnd(){
			if (dashState == DashState.Attack) ExitAction();
		}

		private enum DashState{
			Dash,
			Attack,
		}
	}
}
