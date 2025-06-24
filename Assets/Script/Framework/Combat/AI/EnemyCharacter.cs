namespace Framework.Combat.Runtime{
	public class EnemyCharacter : BaseCharacter{
		public EnemyType enemyType;
		public NavMovementComponent navComp{ get; set; }
		public override void PossessedBy(BaseController newController){
			base.PossessedBy(newController);
			navComp = GetComponent<NavMovementComponent>();
		}

	}
}
