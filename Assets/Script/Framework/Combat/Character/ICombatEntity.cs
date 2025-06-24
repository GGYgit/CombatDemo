using System;
using UnityEngine;

namespace Framework.Combat.Runtime{
	public interface ICombatEntity{

		GameObject gameObject{ get; }
		Transform transform{ get; }
		/// <summary>
		/// 获取战斗阵营
		/// </summary>
		CombatTeamType GetEntityCombatTeam();

		/// <summary>
		/// 返回一个角色的半径，用于距离计算
		/// </summary>
		float GetEntityExtentRadius();
		/// <summary>
		/// 实体是否存活
		/// </summary>
		bool IsEntityAlive();
		/// <summary>
		/// 阵营分配
		/// </summary>
		void AssignTeamToEntity(CombatTeamType inCombatTeamType);

		/// <summary>
		/// 对目标角色造成伤害
		/// </summary>
		void DealDamage(ICombatEntity targetEntity,DamageInfo damageInfo);

		/// <summary>
		/// 对该角色造成伤害
		/// </summary>
		/// <param name="damageInfo"></param>
		void TakeDamage(DamageInfo damageInfo);

		/// <summary>
		/// 当被攻击命中时，未执行伤害计算
		/// </summary>
		/// <returns>是否允许造成伤害</returns>
		public bool CheckCanTakeDamage();


		/// <summary>
		/// 受击回调
		/// </summary>
		public Action<DamageInfo> OnTakeDamageEvent{ get; }
		/// <summary>
		/// 命中其他角色回调
		/// </summary>
		public Action<DamageInfo> OnDealDamageEvent{ get; }
	}
}
