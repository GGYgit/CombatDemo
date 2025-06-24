using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Combat.Runtime{
	[Serializable]
	public class WeaponSwingAnimation : IGeometryAnimEffect{
		[ChildGameObjectsOnly, Required]
		public string weaponTransformName = "WeaponSocket";
		public WeaponAnimationConfig config;

		private Vector3 idlePos;
		private Quaternion idleRot;
		private Transform weaponTransform;
		private Transform root;

		public float Duration => config.preDuration + config.activeDuration + config.postDuration;

		public void Initialize(Transform root){
			if (this.root != null) return;
			this.root = root;
			weaponTransform = root.FindChildByNameRecursive(weaponTransformName);
			if (weaponTransform != null){
				idlePos = weaponTransform.localPosition;
				idleRot = weaponTransform.localRotation;
			}
		}

		public void Evaluate(float time){
			if (weaponTransform == null || config == null) return;
			MotionStage stage = MotionStage.None;
			float stageTime = time;
			if (time >= config.preDuration + config.activeDuration){
				stage = MotionStage.PostAttack;
				stageTime -= config.preDuration + config.activeDuration;
			} else if (time >= config.preDuration){
				stage = MotionStage.Attacking;
				stageTime -= config.preDuration;
			} else{
				stageTime = time;
				stage = MotionStage.PreAttack;
			}
			switch (stage){
				case MotionStage.PreAttack:
					EvaluateAnimation(MotionStage.PreAttack, stageTime); //前摇阶段把武器从待机移动至攻击开始时的位置
					break;
				case MotionStage.Attacking:
					EvaluateAnimation(MotionStage.Attacking, stageTime); // 挥动或突刺武器
					break;
				case MotionStage.PostAttack:
					EvaluateAnimation(MotionStage.PostAttack, stageTime); //后摇，结束过度回待机
					break;
			}
		}

		private void EvaluateAnimation(MotionStage stage, float stageTime){
			if (stage == MotionStage.PreAttack){
				float t = stageTime / config.preDuration;
				GetAttackingPositionAndRotation(0, out Vector3 pos, out Quaternion rot);
				weaponTransform.transform.localPosition =
					Vector3.Lerp(idlePos, pos, t);
				weaponTransform.transform.localRotation =
					Quaternion.Lerp(idleRot, rot, t);
			} else if (stage == MotionStage.Attacking){ //
				float t = Mathf.Clamp01(stageTime / config.activeDuration);
				GetAttackingPositionAndRotation(t, out Vector3 pos, out Quaternion rot);
				weaponTransform.transform.localPosition = pos;
				weaponTransform.transform.localRotation = rot;
			} else if (stage == MotionStage.PostAttack){
				float t = Mathf.Clamp01(stageTime / config.postDuration);
				Vector3 returnPos = idlePos;
				Quaternion returnRot = idleRot;
				weaponTransform.transform.localPosition =
					Vector3.Lerp(weaponTransform.transform.localPosition, returnPos, t);
				weaponTransform.transform.localRotation =
					Quaternion.Lerp(weaponTransform.transform.localRotation, returnRot, t);
			}
		}

		private void GetAttackingPositionAndRotation(float t, out Vector3 attackingPos, out Quaternion attackingRot){
			float evaluatedT = config.swingCurve.Evaluate(t);
			Vector3 offset = Vector3.zero;
			switch (config.swingType){
				case SwingType.Arc:{
					float startAngle = config.swingStartAngle * Mathf.Deg2Rad;
					float endAngle = config.swingEndAngle * Mathf.Deg2Rad;
					float angle = Mathf.Lerp(startAngle, endAngle, evaluatedT);
					offset = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * config.swingRadius;
					break;
				}
				case SwingType.Thrust:{
					float dist = Mathf.Lerp(config.thrustStartDistance, config.thrustEndDistance, evaluatedT);
					offset = Vector3.forward * dist;
					break;
				}
				case SwingType.Overhead:{
					// 从上往下劈：垂直圆弧（YZ 平面）
					float angle = Mathf.Lerp(config.swingStartAngle, config.swingEndAngle, evaluatedT);
					Quaternion rot = Quaternion.Euler(angle, 0, 0);
					offset = rot * Vector3.forward * config.swingRadius;
					break;
				}
				case SwingType.Cross:{
					// 双段交叉斩
					float halfT = evaluatedT * 2f;
					float angle;
					float verticalOffset;
					if (halfT < 1f){
						// 第一段：左下 -> 右上
						angle = Mathf.Lerp(config.swingStartAngle, config.swingEndAngle, halfT);
					} else{
						// 第二段：右下 -> 左上
						halfT -= 1f;
						angle = Mathf.Lerp(config.swingEndAngle, config.swingStartAngle, halfT);
					}
					verticalOffset = Mathf.Lerp(-config.crossHeight, config.crossHeight, halfT);
					Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
					offset = rot * (Vector3.forward * config.swingRadius) + Vector3.up * verticalOffset;
					break;
				}
				case SwingType.Spin:{
					// 原地旋转挥舞，绕 Y 轴一圈，从左手开始
					float angle = Mathf.Lerp(config.swingStartAngle, config.swingEndAngle, evaluatedT);
					// angle -= 90f;
					Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
					offset = rot * (Vector3.forward * config.swingRadius);
					break;
				}
			}
			Vector3 localCenter = Vector3.up * config.swingHeight;
			Vector3 localPos = localCenter + offset;
			Vector3 localDir = offset == Vector3.zero ? Vector3.forward : offset.normalized;
			attackingPos = localPos;
			attackingRot = Quaternion.LookRotation(localDir, Vector3.up);
		}


		public void Restore(){
			if (weaponTransform == null) return;
			weaponTransform.localPosition = idlePos;
			weaponTransform.localRotation = idleRot;
		}
	}

	[Serializable]
	public class WeaponAnimationConfig{
		public SwingType swingType = SwingType.Arc;
		public float swingHeight = 1.5f;
		public float swingRadius = 1.5f;

		public float preDuration = 0.3f; //前摇持续时间
		public float activeDuration = 0.3f; //激活持续时间
		public float postDuration = 0.2f; //后摇持续时间
		public AnimationCurve
			swingCurve = new(new Keyframe(0, 0.25f), new Keyframe(0.5f, 1f), new Keyframe(1, 0.25f)); //挥动动画曲线
		// Arc 挥动
		public float swingStartAngle = -60f; // 默认左→右
		public float swingEndAngle = 60f;

		// Thrust 推刺
		public float thrustStartDistance = 0.5f;
		public float thrustEndDistance = 1.5f;

		// 交叉斩
		public float crossHeight = 0.5f;
	}

	public enum SwingType{
		Arc, // 弧形（左→右 or 右→左）
		Thrust, // 突刺
		Overhead, //劈砍
		Cross, //交叉斩
		Spin, //旋转
	}

	public enum MotionStage{
		None,
		PreAttack,
		Attacking,
		PostAttack,
	}
}
