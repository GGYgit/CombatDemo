using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 受击动作类
	/// </summary>
	public class HitAction : BaseAction{
		public float knockbackDuration = 0.3f;
		//击退曲线，默认值模拟开始时有个爆发段，持续滑动然后开始减速的效果
		public AnimationCurve knockbackCurve = new AnimationCurve(
			new Keyframe(0f, 0f, 0f, 4f),
			new Keyframe(0.15f, 0.9f, 0f, 0f),
			new Keyframe(0.5f, 1f, 0f, -2f),
			new Keyframe(1f, 0f, -4f, 0f));

		private Vector3 knockbackDir;
		private float knockbackTime = 0f;
		private float knockbackDistance = 0f;
		private bool isKnockback = false;

		private NavMovementComponent navComp;

		protected override void OnActivated(object userdata = null){
			base.OnActivated(userdata);
			DamageInfo damageInfo = userdata as DamageInfo;
			Vector3 direction = damageInfo.damageReceiver.transform.position -
				damageInfo.damageCauser.transform.position;
			knockbackDir = direction.normalized;
			knockbackDistance = damageInfo.knockBackDistance;
			knockbackTime = 0f;
			isKnockback = true;
			navComp = OwnerChar.GetCharComponent<NavMovementComponent>();
			if (navComp){
				navComp.Stop();
			}
		}

		protected override void OnDeactivated(){
			base.OnDeactivated();
		}

		/// <summary>
		/// 受击时是否使用角色DeltaTime，
		/// </summary>
		/// <param name="deltaTime"></param>
		protected override void OnTick(float deltaTime){
			base.OnTick(deltaTime);
			if (!isKnockback) return;
			knockbackTime += deltaTime;
			float t = knockbackTime / knockbackDuration;
			if (t >= 1f){
				isKnockback = false;
				ExitAction();
				return;
			}
			float displacement = knockbackCurve.Evaluate(t) * knockbackDistance * deltaTime / knockbackDuration;
			OwnerChar.Move(knockbackDir * displacement);
		}
	}
}
