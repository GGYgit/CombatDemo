using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Combat.Runtime{
	public class EnemyController : BaseController, FightCircleSubject{
#region Serialize Field
		public AIPerceptionComponent perceptionComp;
		public AIState defaultState = AIState.Wait;
		public CombatStyle combatStyle = CombatStyle.Melee;
		[Header("Combat")]
		public AICombatBehaviorComponent combatBehaviorComp;
#endregion

		private Fsm<EnemyController> fsm;
		public NavMovementComponent Movement{ get; private set; }
		public EnemyCharacter EnemyChar{ get; set; }
		public AICombatStateType CombatStateType{ get; set; }

#region Debug
		[Title("Debug")]
		[ShowInInspector]
		private string CurrentState => fsm?.CurrentState?.GetType().Name;
		[ShowInInspector]
		private string CombatState => CombatStateType.ToString();
		[ShowInInspector]
		private string Behaviour => fsm?.GetState<CombatState>()?.CurrentSequence?.Current?.GetType().ToString();
		[ShowInInspector]
		private string CircleType => fsm?.GetState<CombatState>()?.CircleType.ToString();

		private void OnDrawGizmosSelected(){
			perceptionComp.OnGizmos(gameObject);
		}
#endregion

		protected override void OnPossess(BaseCharacter character){
			base.OnPossess(character);
			EnemyChar = character as EnemyCharacter;
			Movement = GetComponent<NavMovementComponent>();
			fsm = Fsm<EnemyController>.Create(string.Empty, this,
				new BaseEnemyState[]{new IdleState(), new PatrolState(), new CombatState()});
			fsm.Start<PatrolState>();
			perceptionComp.StartListening(this);
			perceptionComp.OnTargetPerceptionUpdated += OnTargetPerceptionUpdated;
			perceptionComp.OnLostTarget += OnLostTarget;
		}

		protected override void OnUnPossess(){
			GetTarget()?.GetCharComponent<FightingCircle>().Unregister(this);
			base.OnUnPossess();
			perceptionComp.StopListening();
			perceptionComp.OnTargetPerceptionUpdated -= OnTargetPerceptionUpdated;
			perceptionComp.OnLostTarget -= OnLostTarget;
		}

		void Update(){
			if (OwnerCharacter == null) return;
			float deltaTime = OwnerCharacter.DeltaTime;
			fsm.Update(deltaTime);
			perceptionComp.Tick(deltaTime);
		}

#region Perception
		private void OnTargetPerceptionUpdated(GameObject target){
			BaseCharacter baseChar = target.GetComponent<BaseCharacter>();
			if (baseChar == null) return;
			TargetComp.SetCurrentTarget(baseChar);
		}

		/// <summary>
		/// todo 丢失目标的处理
		/// </summary>
		private void OnLostTarget(){
			TargetComp.SetCurrentTarget(null);
		}
#endregion


#region FightCircle
		public Vector3 GetPosition(){
			return OwnerCharacter.GetPosition();
		}

		public CombatStyle GetCombatStyle(){
			return combatStyle;
		}


		public bool CanAttack(){
			return !OwnerCharacter.ActionComp.IsPerformingAction;
		}

		public float GetFightPriority(){
			return 1f;
		}

		public void OnCircleUpdate(CircleType circleType){
		}
#endregion
	}
}
