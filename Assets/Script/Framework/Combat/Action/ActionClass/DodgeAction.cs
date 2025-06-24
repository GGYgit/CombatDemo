using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 闪避动作，会添加无敌状态
	/// </summary>
	public class DodgeAction : BaseAction{
		public float dodgeSpeed = 15;
		public float dodgeDuration = 0.6f;
		public float invincibleDuration = 0.4f;
		public Vector2 defaultDodgeDirection = new Vector2(0, 1); //x左右，y前后
		public AnimationConfig animationConfig;
		private Vector3 dodgeDirection;
		private float time;
		//速度曲线默认值模拟开始时有个爆发段，持续滑动然后开始减速的效果
		public AnimationCurve speedCurve = new AnimationCurve(
			new Keyframe(0f, 0f, 0f, 4f),
			new Keyframe(0.15f, 0.9f, 0f, 0f),
			new Keyframe(0.5f, 1f, 0f, -2f),
			new Keyframe(1f, 0f, -4f, 0f));

		protected override void OnActivated(object userdata = null){
			time = 0f;
			Vector2 inputDir = OwnerChar.MovementComp.GetMoveInput();
			if (inputDir == Vector2.zero){
				dodgeDirection = defaultDodgeDirection.normalized.GetCameraRelativeDirection();
			} else dodgeDirection = inputDir.normalized.GetCameraRelativeDirection();
			OwnerChar.StatusComp.AddStatus(CharacterStatusType.Invincible, invincibleDuration);
			ExecuteAction(animationConfig);
		}

		protected override void OnDeactivated(){
			OwnerChar.StatusComp.RemoveStatus(CharacterStatusType.Invincible);
		}

		protected override void OnTick(float deltaTime){
			time += deltaTime;
			float t = Mathf.Clamp01(time / dodgeDuration);
			float evaluatedSpeed = dodgeSpeed * speedCurve.Evaluate(t);
			Vector3 move = dodgeDirection * evaluatedSpeed * deltaTime;
			OwnerChar.Move(move);
			if (time >= dodgeDuration){
				ExitAction();
			}
		}
	}
}
