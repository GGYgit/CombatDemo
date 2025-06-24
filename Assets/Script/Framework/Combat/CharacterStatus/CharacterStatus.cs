namespace Framework.Combat.Runtime{
	/// <summary>
	/// 角色状态/Buff
	/// </summary>
	public class CharacterStatus{
		public CharacterStatusType type;
		public float duration;
		public float elapsed;
		public bool IsExpired => elapsed >= duration;

		public CharacterStatus(CharacterStatusType type, float duration){
			this.type = type;
			this.duration = duration;
			this.elapsed = 0f;
		}

		public void Tick(float deltaTime){
			elapsed += deltaTime;
		}
	}


	public enum CharacterStatusType{
		Invincible,
	}
}
