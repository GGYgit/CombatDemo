using System;
using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 向前滑行事件
	/// </summary>
	[Serializable]
	public class SlideForwardEvent : BaseActionEvent{
		public AnimationCurve motionCurve = new(new Keyframe(0, 0.25f), new Keyframe(0.5f, 1f), new Keyframe(1, 0.25f));
		public float maxSpeed = 20f;
		private float duration;
		private BaseCharacter ownerChar;

		public override void OnEventStarted(BaseAction performingAction){
			base.OnEventStarted(performingAction);
			ownerChar = performingAction.OwnerChar;
			duration = 0;
		}


		public override void OnTick(float deltaTime){
			duration += deltaTime;
			float durationScale = duration / length;
			float speedScale = motionCurve.Evaluate(durationScale);
			float speed = maxSpeed * speedScale;
			ownerChar.MovementComp.Move((ownerChar.GetForwardVector() * (speed * deltaTime)));
		}
	}
}
