using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Combat.Runtime{
	public enum CameraEventState{
		FadeIn,
		FadeOut,
	}

	public enum ECameraEventOperationMode{
		[LabelText("叠加")] Add,
		[LabelText("覆盖")] Override,
	}


	[System.Serializable]
	public class CameraEventData{
		[VerticalGroup("事件名"), HideLabel] public string Name;
		[VerticalGroup("偏移"), HideLabel] public Vector3 Offset;
		[VerticalGroup("角度偏移"), HideLabel] public Vector3 RotationOffset;
		[VerticalGroup("淡入速度"), HideLabel, Tooltip("淡入速度")]
		public float interpSpeed;
		[VerticalGroup("淡出速度"), HideLabel, Tooltip("淡出速度")]
		public float fadeOutInterpSpeed;
		[VerticalGroup("计算模式"), HideLabel, Tooltip("淡出速度")]
		public ECameraEventOperationMode operationMode = ECameraEventOperationMode.Add;
		[VerticalGroup("震动"), TableColumnWidth(28, false), HideLabel]
		public bool shake;
		[ShowIf("shake"), VerticalGroup("震动持续时间"), HideLabel, Tooltip("震动持续时间，通用数值：小幅度震动 0.1 中等幅度 0.3 大幅度 0.5")]
		[Range(0, 1)]
		public float shakeDuration;
		[ShowIf("shake"), VerticalGroup("震动幅度"), HideLabel, Tooltip("震动幅度比例，通用值是1")] [Range(0, 3f)]
		public float shakeRange = 1f;
		[ShowIf("shake"), VerticalGroup("持续震动"), TableColumnWidth(56, false), HideLabel]
		public bool shakeLooping;
		public float fov;


		public float Weight => transformWeight;
		[NonSerialized] public CameraEventState state;

		private float transformWeight;
		private float fadeoutFactor = 1;

		public CameraEventData(){
			interpSpeed = 1;
			shakeRange = 1;
		}

		public CameraEventData(string name){
			Name = name;
		}

		public void ResetToFadeIn(){
			state = CameraEventState.FadeIn;
			transformWeight = 0;
			fadeoutFactor = 1;
		}

		public void OnExit(){
			state = CameraEventState.FadeOut;
			fadeoutFactor = -1;
		}


		/// <summary>
		/// </summary>
		/// <param name="cameraState"></param>
		/// <param name="deltaTime"></param>
		public void CalculateModification(CameraState cameraState, float deltaTime){
			float speed = interpSpeed;
			if (state == CameraEventState.FadeOut) speed = fadeOutInterpSpeed;
			transformWeight = Mathf.Clamp01(transformWeight + deltaTime * speed * fadeoutFactor);
			// Debug.Log(deltaTime * interpSpeed * fadeoutFactor);
			cameraState.right += Offset.x * transformWeight;
			cameraState.height += Offset.y * transformWeight;
			cameraState.defaultDistance += Offset.z * transformWeight;
			cameraState.rotationOffSet += RotationOffset * transformWeight;
			cameraState.fov += fov * transformWeight;
		}
	}

	[Serializable]
	public class CameraControlOverride{
		public float fadeInValue;
		public float fadeOutValue;
		[ReadOnly,HideInEditorMode] public CameraEventState state;

		public float fadeInSpeed = 5;
		public float fadeOutSpeed = 6;


		public float Weight => transformWeight;
		private float transformWeight;
		private bool infiniteDuration = true;
		private float durationTime;
		private float duration;


		public void SetDuration(float duration){
			if (duration <= 0) infiniteDuration = true;
			else{
				infiniteDuration = false;
				durationTime = duration;
				this.duration = 0;
			}
		}
		public float GetSpeed(){
			return state == CameraEventState.FadeIn ? fadeInSpeed : fadeOutSpeed;
		}

		public float GetValue(){
			return state == CameraEventState.FadeIn ? fadeInValue : fadeOutValue;
		}


		public void Enter(){
			state = CameraEventState.FadeIn;
			transformWeight = 0;
		}

		public void Exit(){
			state = CameraEventState.FadeOut;
			transformWeight = 0;
		}

		public float CalculateModification(float overrideValue, float deltaTime){
			if (!infiniteDuration){
				duration += deltaTime;
				if (duration > durationTime){
					Exit();
					duration = 0;
					infiniteDuration = true;
					return overrideValue;
				}
			}
			transformWeight = Mathf.Clamp01(transformWeight + deltaTime * GetSpeed());
			overrideValue = Mathf.Lerp(overrideValue, GetValue(), transformWeight);
			return overrideValue;
		}
	}
}
