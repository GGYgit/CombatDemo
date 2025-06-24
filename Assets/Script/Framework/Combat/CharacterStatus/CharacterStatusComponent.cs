using System.Collections.Generic;
using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 角色状态/Buff组件
	/// </summary>
	public class CharacterStatusComponent : CharacterComponent{
		private List<CharacterStatus> statusList;

		protected override void OnInit(BaseCharacter character){
			base.OnInit(character);
			statusList = new List<CharacterStatus>();
		}

		public void AddStatus(CharacterStatusType type, float duration){
			var existing = statusList.Find(s => s.type == type);
			if (existing != null){
				existing.duration = Mathf.Max(existing.duration, duration);
				existing.elapsed = 0f;
			} else{
				statusList.Add(new CharacterStatus(type, duration));
			}
		}

		public void RemoveStatus(CharacterStatusType type){
			statusList.RemoveAll(s => s.type == type);
		}

		public bool HasStatus(CharacterStatusType type){
			return statusList.Exists(s => s.type == type && !s.IsExpired);
		}

		public void Tick(float deltaTime){
			for (int i = statusList.Count - 1; i >= 0; i--){
				statusList[i].Tick(deltaTime);
				if (statusList[i].IsExpired){
					statusList.RemoveAt(i);
				}
			}
		}

		public void ClearAll(){
			statusList.Clear();
		}
	}
}
