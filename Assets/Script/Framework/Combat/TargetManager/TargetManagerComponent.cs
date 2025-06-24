using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 目标组件，管理当前目标
	/// </summary>
	[Serializable]
	public class TargetManagerComponent : CharacterComponent{
		public UnityEvent<GameObject> onNewTarget;
		public UnityEvent onLostTarget;
		[ShowInInspector]
		protected BaseCharacter currentTarget;
		protected float desiredDistance;
		protected Vector3 desiredPosition; //期望攻击位置



		/// <summary>
		/// 获取与当前目标的期望距离，因为并不总是希望玩家与敌人紧贴
		/// </summary>
		public float GetDesiredDistance(){
			if (HasTarget())
				return desiredDistance + Owner.GetEntityExtentRadius() + currentTarget.GetEntityExtentRadius();
			return 0;
		}

		/// <summary>
		/// 获取期望的攻击方向
		/// </summary>
		public Vector3 GetDesiredDirection(){
			if (HasTarget()){
				Vector3 distanceVector = currentTarget.GetPosition() - Owner.GetPosition();
				distanceVector.y = 0;
				distanceVector = distanceVector.normalized;
				return distanceVector;
			}
			return Owner.GetForwardVector();
		}

		/// <summary>
		/// 返回最近可攻击目标
		/// </summary>
		public virtual BaseCharacter GetNearestTarget(float range){
			return GetCurrentTarget();
		}

		/// <summary>
		/// 获取最远的目标
		/// </summary>
		public virtual BaseCharacter GetFarthestTarget(float maxRange){
			return GetCurrentTarget();
		}

		public BaseCharacter GetCurrentTarget(){
			return currentTarget;
		}

		/// <summary>
		/// 设置目标
		/// </summary>
		public virtual void SetCurrentTarget(BaseCharacter target){
			SetTarget(target);
		}


		public void SetDesiredDistance(float distance){
			this.desiredDistance = distance;
		}

		/// <summary>
		/// 是否存在目标
		/// </summary>
		public bool HasTarget(){
			return currentTarget != null;
		}


		protected void SetTarget(BaseCharacter target){
			if (currentTarget == target) return;
			if (target != null){
				onNewTarget.Invoke(target.gameObject);
			} else{
				onLostTarget.Invoke();
			}
			currentTarget = target;
		}
	}
}
