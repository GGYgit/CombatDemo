using UnityEngine;

namespace Framework.Combat.Runtime{
	public class MovementComponent : CharacterComponent{
		protected Vector2 moveInput;
		protected bool gravityEnabled = true;


		public Vector2 GetMoveInput() => moveInput;


		public void DisableGravity(){
			gravityEnabled = false;
		}

		public void EnableGravity(){
			gravityEnabled = true;
		}

		/// <summary>
		/// 位移输入
		/// </summary>
		public virtual void MoveInput(Vector2 input){
			moveInput = input;
		}

		/// <summary>
		/// 位移
		/// </summary>
		/// <param name="offset"></param>
		public virtual void Move(Vector3 offset){
			transform.position += offset;
		}

		/// <summary>
		/// 移动到目标点
		/// </summary>
		/// <param name="destination"></param>
		public virtual void MoveTo(Vector3 destination){
		}

		/// <summary>
		/// 是否可移动
		/// </summary>
		/// <returns></returns>
		public virtual bool CanMove(){
			if (owner.ActionComp && owner.ActionComp.IsPerformingAction) return false;
			return true;
		}


		/// <summary>
		/// 转向目标点，仅水平旋转
		/// </summary>
		/// <param name="point"></param>
		/// <param name="rotationSpeed"></param>
		public virtual void RotateToPoint(Vector3 point, float rotationSpeed){
			Vector3 dir = point - transform.position;
			dir.y = 0;
			if (dir.sqrMagnitude < 0.001f)
				return;
			Quaternion targetRotation = Quaternion.LookRotation(dir);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
		}

		/// <summary>
		/// 指定速度转向，仅水平转向，Y轴不会转
		/// </summary>
		/// <param name="direction">方向</param>
		/// <param name="rotationSpeed">速度180为立即转向</param>
		public virtual void RotateToDirection(Vector3 direction, float rotationSpeed){
			direction.y = 0f;
			if (direction.normalized.magnitude == 0){
				direction = transform.forward;
			}
			var euler = transform.rotation.eulerAngles.NormalizeAngle();
			var targetEuler = Quaternion.LookRotation(direction.normalized).eulerAngles.NormalizeAngle();
			euler.y = Mathf.LerpAngle(euler.y, targetEuler.y, rotationSpeed / 180);
			transform.rotation = Quaternion.Euler(euler);
		}
	}
}
