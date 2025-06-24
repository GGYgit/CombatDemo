using System.Collections.Generic;
using UnityEngine;

namespace Framework.Combat.Runtime{
	[CreateAssetMenu(menuName = "Config/ActionSet")]
	public class ActionSet : ScriptableObject{
		[SerializeField, SerializeReference]
		public List<BaseAction> actions = new List<BaseAction>();


		public bool GetActionByTag(string actionTag, out BaseAction outAction){
			outAction = actions.Find(state => state.actionTag == actionTag);
			return outAction != null;
		}

		public List<BaseAction> GetActions(){
			List<BaseAction> list2 = new List<BaseAction>(actions);
			return list2;
		}
	}
}
