using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.Combat.Runtime{
	public static class CharacterExtensions{
		private static RaycastHit[] raycastHits = new RaycastHit[25];

		public static float CalculateDistanceBetweenCharactersExtents(this BaseCharacter characterA,
			BaseCharacter characterB){
			if (characterA && characterB){
				float extentRadiusA = characterA.GetEntityExtentRadius();
				float extentRadiusB = characterB.GetEntityExtentRadius();
				float dist = characterA.GetDistanceTo(characterB.transform);
				return dist - extentRadiusA - extentRadiusB;
			}
			return -1f;
		}

		// private static float ScreenWeight = 1.5f; //屏幕中心权重
		// private static float DistanceWeight = 5f; //距离权重

		/// <summary>
		/// 取基于屏幕中心与距离混合的可能目标
		/// </summary>
		/// <param name="range">最大距离</param>
		/// <param name="screenWeight">屏幕中心权重</param>
		/// <param name="distanceWeight">距离权重</param>
		/// <param name="orderByDescending">是否倒序</param>
		public static List<BaseCharacter> GetPossibleTargets(this BaseCharacter character, float range,
			float screenWeight = 1f, float distanceWeight = 10f,
			bool orderByDescending = false){
			Camera mainCam = Camera.main;
			Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
			Collider[] hits = Physics.OverlapSphere(character.GetPosition(), range, LayerManager.Character);
			List<(BaseCharacter target, float score)> candidates = new();
			foreach (var hit in hits){
				Transform enemy = hit.transform;
				if (enemy == character.transform) continue;
				if (!isCharacterValid(enemy)) continue;
				Vector3 worldPos = enemy.position;
				Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);
				if (!IsScreenVisible(screenPos)) continue;
				// 屏幕中心距离超过屏幕宽度一半视为不可见
				float distToScreenCenter = Vector2.Distance(new Vector2(screenPos.x, screenPos.y), screenCenter);
				// 遮挡检测
				Vector3 camPos = mainCam.transform.position;
				Vector3 dir = (worldPos - camPos).normalized;
				float distToTarget = Vector3.Distance(camPos, worldPos);
				if (Physics.Raycast(camPos, dir, distToTarget, LayerManager.Obstacles)) continue;

				// 得分
				float worldDist = Vector3.Distance(character.GetPosition(), worldPos);
				float score = (distToScreenCenter * screenWeight) + (worldDist * distanceWeight);
				BaseCharacter enemyChar = enemy.GetComponent<BaseCharacter>();
				candidates.Add((enemyChar, score));
			}
			if (orderByDescending){
				return candidates.OrderByDescending(p => p.score).Select(p => p.target).ToList();
			} else return candidates.OrderBy(p => p.score).Select(p => p.target).ToList();
		}

		private static bool IsScreenVisible(Vector3 screenPos){
			return screenPos.z > 0 &&
				screenPos.x >= 0 && screenPos.x <= Screen.width &&
				screenPos.y >= 0 && screenPos.y <= Screen.height;
		}

		public static bool isCharacterValid(this Transform other){
			var entity = other.GetComponent<BaseCharacter>();
			if (entity == null){
				return false;
			}
			if (entity.IsEntityAlive()){
				return true;
			}
			return false;
		}
	}
}
