using System;

namespace Framework.Combat.Runtime{
	[Serializable]
	public abstract class AIBehaviour{
		protected EnemyController controller;
		protected EnemyCharacter enemyChar;
		public virtual void Start(EnemyController enemyController){
			controller = enemyController;
			enemyChar = enemyController.EnemyChar;
		}
		/// <summary>
		/// 行为的Tick，返回运行状态，返回失败中断执行，返回成功继续执行队列
		/// </summary>
		/// <returns>运行状态</returns>
		public abstract BehaviourState Tick(EnemyController enemyController, float deltaTime);
		public abstract void End(EnemyController enemyController);
	}

	public enum BehaviourState{
		Running,
		Success,
		Failure,
	}
}
