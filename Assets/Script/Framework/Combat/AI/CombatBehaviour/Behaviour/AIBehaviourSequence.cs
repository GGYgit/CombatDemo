using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Combat.Runtime{
	[Serializable]
	public class AIBehaviourSequence{
		[SerializeReference]
		public List<AIBehaviour> actions = new List<AIBehaviour>();
		private AIBehaviour current;
		private int currentIndex;
		private BehaviourState behaviourState;
		private EnemyController enemyController;

		public bool IsFinished => behaviourState == BehaviourState.Success || behaviourState == BehaviourState.Failure;
		public BehaviourState State => behaviourState;
		public AIBehaviour Current => current;

		/// <summary>
		/// 开始执行行为序列
		/// </summary>
		/// <param name="controller"></param>
		public void Start(EnemyController controller){
			this.enemyController = controller;
			currentIndex = 0;
			if (actions.Count == 0){
				behaviourState = BehaviourState.Failure;
				return;
			}
			current = actions[currentIndex];
			current.Start(enemyController);
			behaviourState = BehaviourState.Running;
		}


		/// <summary>
		/// 执行下一行为序列
		/// </summary>
		private void NextBehaviour(){
			current?.End(enemyController);
			currentIndex++;
			if (currentIndex >= actions.Count){
				behaviourState = BehaviourState.Success;
				return;
			}
			current = actions[currentIndex];
			current.Start(enemyController);
		}

		public void Update(EnemyController controller,float elapseSeconds){
			if (current != null){
				BehaviourState state = current.Tick(controller, elapseSeconds);
				if (state == BehaviourState.Success){
					NextBehaviour();
				} else if (state == BehaviourState.Failure){ //中断执行
					Interrupt();
				}
			}
		}

		public void Interrupt(){
			current?.End(enemyController);
			behaviourState = BehaviourState.Failure;
		}
	}
}
