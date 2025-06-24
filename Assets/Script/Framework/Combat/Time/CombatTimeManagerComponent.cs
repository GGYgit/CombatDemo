using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Combat.Runtime{
	[Flags]
	public enum EPauseFlag{
		None = 0,
		GamePause = 1 << 1,
	}

	/// <summary>
	/// 参与战斗实体的时间管理
	/// </summary>
	public class CombatTimeManagerComponent : FrameworkComponent{
		public Action<bool> OnFreezeFrameEvent = delegate(bool b){ };
		public Action<bool> OnGamePauseEvent = delegate(bool b){ };

		[SerializeField, ReadOnly]
		private float currentTimescale = 1;
		[SerializeField, ReadOnly]
		private float playerTimeScale = 1;
		[SerializeField, ReadOnly]
		private float enemyTimeScale = 1;
		[SerializeField]
		private TimeScaleModifier freezeFrameModifier;
		[SerializeField]
		private TimeScaleModifier actionSystemModifier; //动作系统使用的时间修改器


		private Dictionary<CombatTeamType, List<BaseCharacter>> charactersDic;
		private float freezeFrameDuration;
		private float actionPerformDuration;
		private bool isPaused;
		private float defaultTimeScale = 1f;
		private EPauseFlag pauseFlag = EPauseFlag.None;

		private List<TimeScaleModifier> timeScaleModifiers;
		public float DeltaTime => Time.deltaTime * currentTimescale;
		public float UnscaleDeltaTime => Time.deltaTime;

		protected override void Awake(){
			base.Awake();
			timeScaleModifiers = new List<TimeScaleModifier>();
			charactersDic =
				new Dictionary<CombatTeamType, List<BaseCharacter>>();
			freezeFrameModifier.OnTick = OnFreezeFrameTick;
			actionSystemModifier.OnTick = OnActionPerformingTick;
		}


		private void Start(){
		}

		public void Pause(){
			if (isPaused) return;
			isPaused = true;
			AddPauseFlag(EPauseFlag.GamePause);
			// SetTimeScale(0);
			OnGamePauseEvent.Invoke(true);
		}

		public void Continue(){
			if (!isPaused) return;
			isPaused = false;
			RemovePauseFlag(EPauseFlag.GamePause);
			OnGamePauseEvent.Invoke(false);
		}

		public void PauseOrContinue(){
			if (isPaused) Continue();
			else Pause();
		}

		/// <summary>
		/// 冻结帧设置
		/// </summary>
		/// <param name="frameCount"></param>
		public void FreezeFrame(int frameCount){
			if (frameCount <= 0) return;
			freezeFrameDuration = frameCount * GameEntry.Main.frameTime;
			AddTimeScaleModifier(freezeFrameModifier);
			OnFreezeFrameEvent.Invoke(true);
		}


		/// <summary>
		/// 提供给动作系统作动作表演的时停
		/// </summary>
		public void ActionPerforming(float time, float timeScale = 0f){
			actionSystemModifier.timescale = timeScale;
			actionPerformDuration = time;
			AddTimeScaleModifier(actionSystemModifier);
		}

		/// <summary>
		/// 添加角色至时间管理组件
		/// </summary>
		public void AddCharacterToManager(BaseCharacter character, CombatTeamType combatTeamType){
			if (!charactersDic.ContainsKey(combatTeamType))
				charactersDic.Add(combatTeamType, new List<BaseCharacter>());
			charactersDic[combatTeamType].Add(character);
			if (combatTeamType == CombatTeamType.Enemy){
				character.SetTimeScale(enemyTimeScale);
			} else{
				character.SetTimeScale(playerTimeScale);
			}
		}

		public void RemoveCharacterFromManager(BaseCharacter character){
			foreach (var charListPair in charactersDic){
				charListPair.Value.Remove(character);
			}
		}


		public bool HasPauseFlag(){
			return this.pauseFlag != EPauseFlag.None;
		}

		private void Update(){
			if (isPaused){
				return;
			}
			for (int i = timeScaleModifiers.Count - 1; i >= 0; i--){
				timeScaleModifiers[i].OnTick.Invoke(UnscaleDeltaTime);
			}
		}

		public void AddTimeScaleModifier(TimeScaleModifier modifier){
			if (timeScaleModifiers.Exists(scaleModifier => scaleModifier.id == modifier.id)) return;
			timeScaleModifiers.Add(modifier);
			UpdateTimescaleByModifier();
		}

		public void RemoveTimeScaleModifier(TimeScaleModifier modifier){
			timeScaleModifiers.Remove(modifier);
			UpdateTimescaleByModifier();
		}


		/// <summary>
		///
		/// </summary>
		private void UpdateTimescaleByModifier(){
			if (timeScaleModifiers.Count <= 0){
				ResetTimeScale();
				return;
			}
			var modifier = timeScaleModifiers[^1];
			var timescale = modifier.timescale;
			SetTimeScale(timescale, modifier.timeScaleEffectTargetType);
		}

		private void OnFreezeFrameTick(float deltaTime){
			freezeFrameDuration -= deltaTime;
			if (freezeFrameDuration <= 0){
				RemoveTimeScaleModifier(freezeFrameModifier);
				OnFreezeFrameEvent.Invoke(false);
			}
		}

		private void OnActionPerformingTick(float deltaTime){
			actionPerformDuration -= deltaTime;
			if (actionPerformDuration <= 0){
				RemoveTimeScaleModifier(actionSystemModifier);
			}
		}

		private void SetTimeScale(float timeScale, ETimeScaleEffectTargetType effectTargetType){
			bool dirtyFlag = false;
			if (effectTargetType.HasFlag(ETimeScaleEffectTargetType.Player)){
				if (Math.Abs(playerTimeScale - timeScale) > MathConstant.Tolerance) dirtyFlag = true;
				playerTimeScale = timeScale;
			}
			if (effectTargetType.HasFlag(ETimeScaleEffectTargetType.Enemy)){
				if (Math.Abs(enemyTimeScale - timeScale) > MathConstant.Tolerance) dirtyFlag = true;
				enemyTimeScale = timeScale;
			}
			if (dirtyFlag){
				UpdateCharacterTimeScale(effectTargetType);
			}
		}


		[Button]
		private void SetAllTimeScale(float timeScale){
			enemyTimeScale = playerTimeScale = currentTimescale = timeScale;
			UpdateCharacterTimeScale(ETimeScaleEffectTargetType.All);
		}

		private void ResetTimeScale(){
			SetAllTimeScale(defaultTimeScale);
		}

		private void UpdateCharacterTimeScale(ETimeScaleEffectTargetType effectTargetType){
			float scale = defaultTimeScale;
			if (effectTargetType.HasFlag(ETimeScaleEffectTargetType.Player)){
				scale = playerTimeScale;
			}
			foreach (var character in charactersDic[CombatTeamType.Player]){
				character.SetTimeScale(scale);
			}
			scale = defaultTimeScale;
			if (effectTargetType.HasFlag(ETimeScaleEffectTargetType.Enemy)){
				scale = enemyTimeScale;
			}
			foreach (var character in charactersDic[CombatTeamType.Enemy]){
				character.SetTimeScale(scale);
			}
		}


		private void AddPauseFlag(EPauseFlag flag){
			this.pauseFlag |= flag;
			OnPauseFlagChange();
		}

		private void RemovePauseFlag(EPauseFlag flag){
			this.pauseFlag &= ~flag;
			OnPauseFlagChange();
		}

		private void OnPauseFlagChange(){
			if (HasPauseFlag()){
				SetAllTimeScale(0);
			} else{
				ResetTimeScale();
				UpdateTimescaleByModifier();
			}
		}
	}
}
