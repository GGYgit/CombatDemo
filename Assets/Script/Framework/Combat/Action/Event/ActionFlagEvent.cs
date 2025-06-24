namespace Framework.Combat.Runtime{
	public class ActionFlagEvent : BaseActionEvent{
		public ActionPhaseFlagType actionPhaseFlag;

		public override void OnEventStarted(BaseAction performingAction){
			performingAction.ActionPhaseFlag |= actionPhaseFlag;
		}


	}
}
