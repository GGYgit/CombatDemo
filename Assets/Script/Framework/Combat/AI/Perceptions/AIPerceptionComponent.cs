using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// AI感知系统组件
	/// </summary>
	[Serializable]
	public class AIPerceptionComponent{
		[SerializeReference]//todo 未实现，感知到目标后进入主导感知模式，基于主导感知决定是否丢失目标
		public AbstractAIPerception dominant;
		[SerializeReference]
		public List<AbstractAIPerception> aiPerceptions = new List<AbstractAIPerception>();

		/// <summary>
		/// 感知更新时调用
		/// </summary>
		public Action<GameObject> OnTargetPerceptionUpdated = delegate{ };
		public Action OnLostTarget = delegate{ };


		[NonSerialized]
		public EnemyController enemyController;


		private bool isChecking = false;

		public void StartListening(EnemyController enemyController){
			this.enemyController = enemyController;
			// if (aiController.BT == null) return;
			// if (aiPerceptions.Count == 0)
			// Debug.Log($"缺少感知", this.aiController.gameObject);
			isChecking = true;
			foreach (var aiPerception in aiPerceptions){
				aiPerception.Start(this);
			}
		}

		public void StopListening(){
			// if (aiController.BT == null) return;
			isChecking = false;
			foreach (var aiPerception in aiPerceptions){
				aiPerception.Stop();
			}
		}


		public void Tick(float deltaTime){
			if (!isChecking) return;
			foreach (var aiPerception in aiPerceptions){
				aiPerception.OnTick(deltaTime);
			}
		}


		/// <summary>
		/// 感知到目标时
		/// </summary>
		public void InvokeOnPerception(AbstractAIPerception aiPerception, GameObject returnedObject){
			OnTargetPerceptionUpdated.Invoke(returnedObject);
		}

		/// <summary>
		/// 丢失感知时
		/// </summary>
		/// <param name="aiPerception"></param>
		public void InvokeOnLostTarget(AbstractAIPerception aiPerception){
			OnLostTarget.Invoke();
		}


		public void OnGizmos(GameObject gameObject){
			foreach (var aiPerception in aiPerceptions){
				if (aiPerception.drawGizmos)
					aiPerception.OnGizmos(gameObject);
			}
		}
	}
}
