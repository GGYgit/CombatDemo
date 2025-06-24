using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Framework.Combat.Runtime{
	[System.Serializable]
	public class CameraState{
		public string Name;
		public float forward;
		public float right;
		// public float Distance;
		public float maxDistance;
		public float minDistance;
		public float height;
		public float smooth = 10f; //状态之间切换的平滑度
		public float smoothDamp = 0f; //相机移动的平滑阻尼
		public float rotationSmooth = 10f;
		// public float XSensitivity;
		// public float YSensitivity;
		public float yMinLimit;
		public float yMaxLimit;
		public float xMinLimit;
		public float xMaxLimit;

		public Vector3 rotationOffSet;
		public float cullingHeight;
		public float cullingMinDist;
		public float fov;
		public bool useZoom;
		public Vector2 fixedAngle;
		public List<LookPoint> lookPoints;
		public CameraMode cameraMode;


#region UserSetting
		public float Distance => defaultDistance + userDistance;
		[SerializeField, FormerlySerializedAs("defaultDistance")]
		public float defaultDistance;
		[NonSerialized] internal float userDistance = 10f;


		public float XSensitivity => xDefaultSensitivity + userSensitivity;
		[SerializeField, FormerlySerializedAs("xMouseSensitivity")]
		public float xDefaultSensitivity;
		public float YSensitivity => yDefaultSensitivity + userSensitivity;
		[SerializeField, FormerlySerializedAs("yMouseSensitivity")]
		public float yDefaultSensitivity;
		[NonSerialized] internal float userSensitivity = 0.5f;

		/// <summary>
		/// 玩家设置摄像机距离
		/// </summary>
		/// <param name="uDistance"></param>
		public void SetUserDistance(float uDistance){
			this.userDistance = uDistance;
		}

		/// <summary>
		/// 玩家设置摄像机灵敏度
		/// </summary>
		/// <param name="sensitivity"></param>
		public void SetUserSensitivity(float sensitivity){
			this.userSensitivity = sensitivity;
		}
#endregion

		public CameraState(string name){
			Name = name;
			forward = -1f;
			right = 0f;
			defaultDistance = 1.5f;
			maxDistance = 3f;
			minDistance = 0.5f;
			height = 0f;
			smooth = 10f;
			smoothDamp = 0f;
			xDefaultSensitivity = 3f;
			yDefaultSensitivity = 3f;
			yMinLimit = -40f;
			yMaxLimit = 80f;
			xMinLimit = -360f;
			xMaxLimit = 360f;
			cullingHeight = 0.2f;
			cullingMinDist = 0.1f;
			fov = 60f;
			useZoom = false;
			forward = 60;
			fixedAngle = Vector2.zero;
			cameraMode = CameraMode.FreeDirectional;
			rotationSmooth = 10;
		}
	}

	[System.Serializable]
	public class LookPoint{
		public string pointName;
		public Vector3 positionPoint;
		public Vector3 eulerAngle;
		public bool freeRotation;
	}

	public enum CameraMode{
		FreeDirectional,
	}

	[System.Serializable]
	public class CameraShakeSettings{
		public int level = 1;
		public float duration = 0.2f;
		public float magnitude = 0.2f;
		public float frequency = 20f;
	}
}
