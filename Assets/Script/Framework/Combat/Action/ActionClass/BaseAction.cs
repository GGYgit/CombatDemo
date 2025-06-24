using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Combat.Runtime{
	[Serializable]
	public abstract class BaseAction{
		public string actionTag;

		protected ActionComponent actionComp;

		private List<ActionEventInfo> actionEventInfos = new List<ActionEventInfo>();
		private float duration = 0;
		private AnimInfo animInfo = null;

		public BaseCharacter OwnerChar{ get; private set; }

		public ActionPhaseFlagType ActionPhaseFlag{ get; set; }


		public void Activated(ActionComponent actionComp, object userdata = null){
			this.actionComp = actionComp;
			animInfo = null;
			ActionPhaseFlag = ActionPhaseFlagType.None;
			OwnerChar = actionComp.Owner;
			OnActivated(userdata);
		}

		public void Deactivated(){
			if (animInfo != null){
				foreach (var effect in animInfo.effects){
					effect.Restore();
				}
				animInfo = null;
			}
			if (actionEventInfos != null){
				foreach (var actionEvent in actionEventInfos){
					if (actionEvent.actionEvent.EndTime <= 0) continue;
					actionEvent.actionEvent.OnActionEnded(this);
				}
				actionEventInfos.Clear();
			}
			OnDeactivated();
		}

		public void ReceiveInput(string newActionTag, ActionPriority actionPriority, object userdata = null){
			OnReceiveInput(newActionTag, actionPriority, userdata);
		}

		public void Tick(float deltaTime){
			duration += deltaTime;
			AnimHandle(duration);
			EventHandle(duration, deltaTime);
			OnTick(deltaTime);
			if (ActionPhaseFlag.HasFlag(ActionPhaseFlagType.Interruptible)){
				if (OwnerChar.MovementComp.GetMoveInput() != Vector2.zero){ //移动打断动作
					ExitAction();
				}
			}
		}


		/// <summary>
		/// 接收到新动作输入
		/// </summary>
		protected virtual void OnReceiveInput(string newActionTag, ActionPriority actionPriority,
			object userdata = null){
		}

		/// <summary>
		/// 动作激活时
		/// </summary>
		/// <param name="userdata"></param>
		protected virtual void OnActivated(object userdata = null){
		}

		/// <summary>
		/// 动作结束时
		/// </summary>
		protected virtual void OnDeactivated(){
		}

		protected virtual void OnTick(float deltaTime){
		}

		/// <summary>
		/// 动画播放结束
		/// </summary>
		protected virtual void OnAnimEnd(){
			ExitAction();
		}

		/// <summary>
		/// 执行动作，处理动作事件、播放动画
		/// </summary>
		/// <param name="animationConfig"></param>
		protected void ExecuteAction(AnimationConfig animationConfig){
			PrepareAnimationInfo(animationConfig);
		}


		/// <summary>
		/// 结束动作
		/// </summary>
		protected void ExitAction(){
			actionComp.ExitAction();
		}


		/// <summary>
		/// 准备动画数据，动画事件
		/// </summary>
		protected void PrepareAnimationInfo(AnimationConfig animationConfig){
			if (animationConfig == null) return;
			duration = 0;
			if (actionEventInfos == null) actionEventInfos = new List<ActionEventInfo>();
			actionEventInfos.Clear();
			foreach (var actionEvent in animationConfig.events){
				if (actionEvent.EndTime <= 0) continue;
				actionEventInfos.Add(new ActionEventInfo(){actionEvent = actionEvent, state = EventState.Wait});
				actionEvent.OnActionStarted(this);
			}
			if (animInfo != null){
				foreach (var effect in animInfo.effects){
					effect.Restore();
				}
			}
			animInfo = animationConfig.animInfo;
			if (animInfo == null){ //纯Action逻辑无动画数据
				return;
			}
			foreach (var effect in animInfo.effects){
				effect.Initialize(OwnerChar.transform);
			}
		}

		/// <summary>
		/// 动画处理
		/// </summary>
		protected void AnimHandle(float time){
			if (animInfo == null) return; //纯Action逻辑无动画数据
			foreach (var effect in animInfo.effects){
				effect.Evaluate(time);
			}
			if (time > animInfo.time){
				OnAnimEnd();
			}
		}

		/// <summary>
		/// 动画事件处理
		/// </summary>
		/// <param name="time"></param>
		/// <param name="deltaTime"></param>
		protected void EventHandle(float time, float deltaTime){
			if (actionEventInfos == null) return;
			foreach (var actionEventInfo in actionEventInfos){
				switch (actionEventInfo.state){
					case EventState.Wait:
						if (time > actionEventInfo.actionEvent.startTime){
							actionEventInfo.state = EventState.Active;
							actionEventInfo.actionEvent.OnEventStarted(this);
						}
						break;
					case EventState.Active:
						if (time > actionEventInfo.actionEvent.EndTime){
							actionEventInfo.state = EventState.Completed;
							actionEventInfo.actionEvent.OnEventEnded(this);
						}
						actionEventInfo.actionEvent.OnTick(deltaTime);
						break;
					case EventState.Completed:
						break;
				}
			}
		}
	}

	public class ActionEventInfo{
		public EventState state = EventState.Wait;
		public BaseActionEvent actionEvent;
	}

	public enum EventState{
		Wait,
		Active,
		Completed,
	}
}
