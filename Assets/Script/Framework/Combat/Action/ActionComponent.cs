using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Combat.Runtime{
	public class ActionComponent : CharacterComponent{
		[SerializeField]
		public ActionSet actionsSet; //基础动作集
		public Action<string> OnReceiveInput = delegate{ };
		public Action<string> OnActionEnd = delegate{ };


		protected int currentPriority = -1;
		[Header("Debug")]
		[ShowInInspector]
		private string performingActionTag;
		[ShowInInspector, InlineProperty]
		private BaseAction performingAction;
		private ActionSet actionSetInst;

		public bool IsLocked{ get; set; }
		public bool IsPerformingAction => performingAction != null;

		protected override void OnInit(BaseCharacter character){
			actionSetInst = Object.Instantiate(actionsSet);
		}

		/// <summary>
		/// 接收动作输入
		/// </summary>
		/// <param name="actionTag"></param>
		public void ReceiveInput(string actionTag, ActionPriority actionPriority, object userdata = null){
			if (IsLocked) return;
			OnReceiveInput.Invoke(actionTag);
			if (performingAction != null && (int) actionPriority <= currentPriority){
				//输入小于等于正在执行动作的优先级，输入交由动作类处理，是否派生或连击什么的。
				performingAction.ReceiveInput(actionTag, actionPriority, userdata);
			} else{
				TriggerAction(actionTag, actionPriority, userdata);
			}
		}

		/// <summary>
		/// 触发一个动作
		/// </summary>
		/// <returns>是否执行成功</returns>
		public bool TriggerAction(string actionTag, ActionPriority actionPriority, object userdata = null){
			if (IsLocked) return false;
			if (CanExecuteAction(actionTag, actionPriority)){
				LaunchAction(actionTag, actionPriority, userdata);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 退出当前动作
		/// </summary>
		public virtual void ExitAction(){
			InternalExitAction();
		}


		/// <summary>
		/// 是否可执行动作判断
		/// </summary>
		public bool CanExecuteAction(string actionTag, ActionPriority actionPriority){
			if (GetActionByTag(actionTag, out BaseAction outAction)){
				if (actionPriority != ActionPriority.Highest){
					if (currentPriority >= (int) actionPriority){
						return false;
					}
				}
				return true;
			}
			return false;
		}


		protected void LaunchAction(string actionTag, ActionPriority actionPriority, object userdata = null){
			if (GetActionByTag(actionTag, out BaseAction action)){
				TerminateCurrentAction();
				performingAction = action;
				performingActionTag = action.actionTag;
				currentPriority = (int) actionPriority;
				performingAction.Activated(this, userdata);
			}
		}

		protected void InternalExitAction(){
			if (performingAction != null){
				TerminateCurrentAction();
			}
		}


		private void TerminateCurrentAction(){
			if (performingAction != null){
				performingAction.Deactivated();
				OnActionEnd.Invoke(performingActionTag);
				performingActionTag = string.Empty;
				performingAction = null;
				currentPriority = -1;
			}
		}


		private bool GetActionByTag(string actionTag, out BaseAction outAction){
			if (actionSetInst.GetActionByTag(actionTag, out outAction)){
				return true;
			}
			return false;
		}

		private void Update(){
			if (IsLocked) return;
			if (performingAction != null){
				performingAction.Tick(owner.DeltaTime);
			}
		}
	}
}
