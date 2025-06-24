using System;

namespace Framework.Combat.Runtime{
	public static class EntityExtensions{
		public static bool IsEnemyTeam(this CombatTeamType combatTeamType, CombatTeamType combatTeamType2){
			if (combatTeamType == combatTeamType2) return false;
			switch (combatTeamType){
				case CombatTeamType.Player:
					if (combatTeamType2 is CombatTeamType.Enemy) return true;
					break;
				case CombatTeamType.Friendly:
					if (combatTeamType2 == CombatTeamType.Enemy) return true;
					break;
				case CombatTeamType.Enemy:
					if (combatTeamType2 is CombatTeamType.Player or CombatTeamType.Friendly) return true;
					break;
				case CombatTeamType.Neutral:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(combatTeamType), combatTeamType, null);
			}
			return false;
		}
	}
}
