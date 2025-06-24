using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 角色基类
	/// </summary>
	public abstract class BaseCharacter : MonoBehaviour, ICombatEntity{
		[SerializeField]
		protected bool fillHealthOnStart = true;
		[SerializeField]
		protected float destroyTimeOnDeath = 3f;
		protected BaseController controller;
		protected float timeScale = 1;
		private CombatTeamType combatTeamType;
		private List<CharacterComponent> charComponents;
		private bool isDead;
		private Collider collider;

		public float DeltaTime => timeScale * Time.deltaTime;
		public Action<DamageInfo> OnTakeDamageEvent{ get; set; } = delegate{ };
		public Action<DamageInfo> OnDealDamageEvent{ get; set; } = delegate{ };
		public AttributeComponent AttributeComp{ get; protected set; }
		public MovementComponent MovementComp{ get; protected set; }
		public ActionComponent ActionComp{ get; protected set; }
		public HitEffectComponent HitEffectComp{ get; protected set; }
		public CharacterStatusComponent StatusComp{ get; protected set; }

		private void Start(){
			var charComps = gameObject.GetComponentsInChildren<CharacterComponent>();
			charComponents = charComps.ToList();
			MovementComp = GetCharComponent<MovementComponent>();
			HitEffectComp = GetCharComponent<HitEffectComponent>();
			AttributeComp = GetCharComponent<AttributeComponent>();
			ActionComp = GetCharComponent<ActionComponent>();
			StatusComp = GetCharComponent<CharacterStatusComponent>();
			collider = GetComponent<Collider>();
			var controller = GetComponent<BaseController>();
			controller?.Possess(this);
			foreach (var charComp in charComps){
				charComp.Init(this);
			}
			GameEntry.Time.AddCharacterToManager(this, combatTeamType);
			if (fillHealthOnStart){
				AttributeComp.SetBaseValue(AttributeConstant.Health,
					AttributeComp.GetCurrentValue(AttributeConstant.MaxHealth));
			}
		}

		private void OnDestroy(){
			GameEntry.Time.RemoveCharacterFromManager(this);
		}


		public void SetTimeScale(float timeScale){
			this.timeScale = timeScale;
		}

		public float GetTimeScale(){
			return timeScale;
		}

		public CombatTeamType GetEntityCombatTeam(){
			return combatTeamType;
		}

		public float GetEntityExtentRadius(){
			return 1f;
		}

		public float GetDistanceTo(Transform target){
			return Vector3.Distance(GetPosition(), target.position);
		}

		public float GetPlanarDistanceTo(Vector3 targetPoint){
			Vector3 selfPos = GetPosition();
			selfPos.y = targetPoint.y = 0;
			return Vector3.Distance(selfPos, targetPoint);
		}

		public Vector3 GetPosition(){
			return transform.position;
		}

		public Vector3 GetForwardVector(){
			return transform.forward;
		}

		public Vector3 GetRightVector(){
			return transform.right;
		}

		public bool IsEntityAlive(){
			return !isDead;
		}

		public void AssignTeamToEntity(CombatTeamType teamType){
			combatTeamType = teamType;
		}

		public void Move(Vector3 velocity){
			if (MovementComp){
				MovementComp.Move(velocity);
			} else{
				transform.position += velocity;
			}
		}

		public virtual void DisableCollider(){
			collider.enabled = false;
		}

		public virtual void EnableCollider(){
			collider.enabled = true;
		}

		/// <summary>
		/// 取角色组件
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetCharComponent<T>() where T : CharacterComponent{
			foreach (var charComponent in charComponents){
				if (charComponent is T component){
					return component;
				}
			}
			return null;
		}

		/// <summary>
		/// 是否可以对该角色造成伤害
		/// </summary>
		public bool CheckCanTakeDamage(){
			if (StatusComp.HasStatus(CharacterStatusType.Invincible)) return false;
			return IsEntityAlive();
		}

		/// <summary>
		/// 对目标角色造成伤害
		/// </summary>
		public void DealDamage(ICombatEntity targetEntity, DamageInfo damageInfo){
			OnDealDamage(targetEntity, damageInfo);
			OnDealDamageEvent.Invoke(damageInfo);
		}

		/// <summary>
		/// 该角色受到伤害
		/// </summary>
		public void TakeDamage(DamageInfo damageInfo){
			AttributeComp.AddCurrentValue(AttributeConstant.Health, -damageInfo.damageValue);
			if (AttributeComp.GetCurrentValue(AttributeConstant.Health) <= 0){
				isDead = true;
				OnDeath(damageInfo);
				return;
			}
			OnTakeDamageEvent.Invoke(damageInfo);
			HitReaction(damageInfo);
		}

		/// <summary>
		/// 角色死亡，Demo不用做池
		/// </summary>
		protected void OnDeath(DamageInfo damageInfo){
			HitEffectComp.TriggerDeadEffect(damageInfo);
			var controller = GetComponent<BaseController>();
			controller?.UnPossess();
			Invoke(nameof(DestroyEntity), destroyTimeOnDeath);
		}

		protected void DestroyEntity(){
			GameObject.Destroy(gameObject);
		}


		/// <summary>
		/// 受击反馈
		/// </summary>
		protected virtual void HitReaction(DamageInfo damageInfo){
			HitEffectComp.TriggerHitEffect(damageInfo);
			if (damageInfo.hitReactionLevel > 0){
				ActionComp.TriggerAction(ActionConstantTag.Hit, ActionPriority.High, damageInfo);
			}
		}

		protected virtual void OnDealDamage(ICombatEntity targetEntity, DamageInfo damageInfo){
		}


		public virtual void PossessedBy(BaseController newController){
			controller = newController;
		}


		public virtual void UnPossessed(){
			controller = null;
		}
	}
}
