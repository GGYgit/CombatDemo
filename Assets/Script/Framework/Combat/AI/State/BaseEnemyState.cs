namespace Framework.Combat.Runtime{
	public abstract class BaseEnemyState:FsmState<EnemyController>{
		protected EnemyController controller;
		protected EnemyCharacter character;

		protected internal override void OnInit(IFsm<EnemyController> fsm){
			base.OnInit(fsm);
			controller = fsm.Owner;
			character = fsm.Owner.EnemyChar;
		}

		protected internal override void OnDestroy(IFsm<EnemyController> fsm){
			base.OnDestroy(fsm);
			controller = null;
			character = null;
		}
	}
}
