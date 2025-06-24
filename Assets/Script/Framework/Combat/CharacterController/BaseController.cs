using JetBrains.Annotations;
using UnityEngine;

namespace Framework.Combat.Runtime{
	public class BaseController : MonoBehaviour{
		public CombatTeamType combatTeam;
		public BaseCharacter OwnerCharacter{ get; private set; }
		public TargetManagerComponent TargetComp{ get; private set; }

		public void Possess(BaseCharacter character){
			if (OwnerCharacter != null) UnPossess();
			OnPossess(character);
		}

		public void UnPossess(){
			if (OwnerCharacter == null) return;
			OnUnPossess();
		}


		/// <summary>
		/// 获取当前目标
		/// </summary>
		[CanBeNull]
		public BaseCharacter GetTarget(){
			return TargetComp?.GetCurrentTarget();
		}


		[CanBeNull]
		public FightingCircle GetTargetFightCircle(){
			var target = GetTarget();
			if (target == null){
				Debug.LogWarning("当前无仇恨目标，无法取FightCircle");
			}
			return target.GetCharComponent<FightingCircle>();
		}

		/// <summary>
		/// 控制器持有角色时
		/// </summary>
		/// <param name="character"></param>
		protected virtual void OnPossess(BaseCharacter character){
			OwnerCharacter = character;
			OwnerCharacter.PossessedBy(this);
			OwnerCharacter.AssignTeamToEntity(combatTeam);
			TargetComp = OwnerCharacter.GetCharComponent<TargetManagerComponent>();
		}

		/// <summary>
		/// 控制器取消持有角色时
		/// </summary>
		protected virtual void OnUnPossess(){
			OwnerCharacter.UnPossessed();
			OwnerCharacter = null;
			TargetComp = null;
		}
	}
}
