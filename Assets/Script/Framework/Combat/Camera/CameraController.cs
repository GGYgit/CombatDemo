using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Framework.Combat.Runtime{
	public class CameraController : MonoBehaviour{
#region inspector properties
		public Transform mainTarget;
		public float smoothBetweenState = 6f;
		public float smoothCameraRotation = 6f;
		public float smoothSwitchSide = 2f;
		public float scrollSpeed = 10f;
		public float mouseSensitivity = 1;
		[Tooltip("剔除层，用于检测是否贴墙")]
		public LayerMask cullingLayer = 1 << 0;
		public float clipPlaneMargin;
		public float checkHeightRadius;
		public bool showGizmos;
		public bool startUsingTargetRotation = true;
		public bool startSmooth = false;
		[FormerlySerializedAs("behindTargetSmoothRotation")]
		public float softLockTargetSmoothRotation = 1f;
		public Vector2 offsetMouse;
		public float farClipPlane = 2000f;
		public bool lockOnVerticalAxis = false;
		public bool isFreezed;
		public float mouseXCorrectSpeed = 5f;
		public float mouseYCorrectSpeed = 10f;
		public float mouseXThreshold = 20f;
		public float mouseYThreshold = 5f;
		public CameraShakeSettings[] shakeSettings = new CameraShakeSettings[0];
		public CameraEventListData cameraEventListData;

#endregion

#region hide properties
		[HideInInspector] public int indexList;
		[HideInInspector] public float offSetPlayerPivot;
		[HideInInspector] public float distance = 5f;
		[HideInInspector] public string currentStateName;
		[HideInInspector] public Transform currentTarget;
		[HideInInspector] public CameraState currentState;
		[HideInInspector] public CameraListData CameraStateList;
		[HideInInspector] public Transform lockTarget;
		[HideInInspector] public Vector2 movementSpeed;
		[HideInInspector] public CameraState lerpState;
		// [HideInInspector] public float followDamp = -1;

		[SerializeField]
		protected bool lockCamera;
		[SerializeField]
		protected float mouseY = 0f;
		[SerializeField]
		protected float mouseX = 0f;

		protected float lockTargetSpeed;
		protected float lockSwitchTargetSpeed;
		protected float lockTargetWeight;
		protected float initialCameraRotation;

		protected bool cameraIsRotating;
		protected float lastRotationTimer;

		protected Vector3 currentTargetPos;
		protected Vector3 lookPoint;
		protected Vector3 current_cPos;
		protected Vector3 desired_cPos;
		protected Vector3 stateFinalPos; //当前状态的相机最终位置，不参与Lerp计算
		protected Vector3 lookTargetAdjust;

		protected float currentHeight;
		protected float currentZoom;
		protected float cullingHeight;
		protected float cullingDistance;
		protected float switchRight;
		protected float currentSwitchRight;
		protected float heightOffset;
		protected float lockOnMaxRange;
		protected bool isInit;
		protected bool useSmooth;
		protected bool isNewTarget;
		protected bool firstStateIsInit;
		protected Quaternion fixedRotation;
		protected Camera targetCamera;

		protected float transformWeight;
		protected float mouseXStart;
		protected float mouseYStart;
		protected Vector3 startPosition;
		protected Quaternion startRotation;

		private Vector3 cameraVelocityDamp;
		private bool firstUpdated;

		protected Transform lookAtTarget;

		protected Vector3 lastLookAtPosition, lastLookAtForward;
		protected Rigidbody selfRigidbody;
		protected float softLockDuration = 0;

		// 当前震动状态
		private float shakeTimer;
		private float shakeDuration;
		private float shakeMagnitude;
		private float shakeFrequency;
		private Vector3 shakeOffset;

		protected Transform TargetLookAt{
			get{
				if (!lookAtTarget){
					lookAtTarget = new GameObject("targetLookAt").transform;
					lookAtTarget.rotation = transform.rotation;
					lookAtTarget.position = mainTarget.position;
				}
				return lookAtTarget;
			}
		}
		public Rigidbody SelfRigidbody{
			get{
				if (!selfRigidbody){
					selfRigidbody = gameObject.AddComponent<Rigidbody>();
					selfRigidbody.isKinematic = true;
					selfRigidbody.interpolation = RigidbodyInterpolation.None;
				}
				return selfRigidbody;
			}
		}
		/// <summary>
		/// 锁定摄像机
		/// </summary>
		public bool LockCamera{
			get{ return lockCamera; }
			set{ lockCamera = value; }
		}

		public Camera TargetCamera{
			get{ return targetCamera; }
		}
#endregion


		public Vector3 StateFinalPos => stateFinalPos;

		protected void OnDrawGizmos(){
			if (showGizmos){
				if (currentTarget){
					var targetPos = new Vector3(currentTarget.position.x, currentTarget.position.y + offSetPlayerPivot,
						currentTarget.position.z);
					Gizmos.DrawWireSphere(targetPos + Vector3.up * cullingHeight, checkHeightRadius);
					Gizmos.DrawLine(targetPos, targetPos + Vector3.up * cullingHeight);
				}
			}
		}

		protected void Start(){
			Init();
		}

		/// <summary>
		/// 摄像机初始化
		/// </summary>
		public void Init(){
			if (mainTarget == null){
				return;
			}
			firstUpdated = true;
			useSmooth = true;
			TargetLookAt.rotation = startUsingTargetRotation ? mainTarget.rotation : transform.rotation;
			TargetLookAt.position = mainTarget.position;
			TargetLookAt.hideFlags = HideFlags.HideInHierarchy;
			startPosition = SelfRigidbody.position;
			startRotation = SelfRigidbody.rotation;
			initialCameraRotation = smoothCameraRotation;
			if (!targetCamera){
				targetCamera = Camera.main;
			}
			currentTarget = mainTarget;
			switchRight = 1;
			currentSwitchRight = 1f;
			mouseXStart = transform.eulerAngles.NormalizeAngle().y;
			mouseYStart = transform.eulerAngles.NormalizeAngle().x;
			if (startSmooth){
				distance = Vector3.Distance(TargetLookAt.position, transform.position);
			} else{
				transformWeight = 1;
			}
			if (startUsingTargetRotation){
				mouseY = currentTarget.eulerAngles.NormalizeAngle().x;
				mouseX = currentTarget.eulerAngles.NormalizeAngle().y;
			} else{
				mouseY = transform.eulerAngles.NormalizeAngle().x;
				mouseX = transform.eulerAngles.NormalizeAngle().y;
			}
			ChangeState("Default", startSmooth);
			currentZoom = currentState.Distance;
			currentHeight = currentState.height;
			currentTargetPos =
				new Vector3(currentTarget.position.x, currentTarget.position.y + offSetPlayerPivot,
					currentTarget.position.z) + currentTarget.transform.up * lerpState.height;
			TargetLookAt.position = currentTargetPos;
			targetCamera.farClipPlane = farClipPlane;
			targetCamera.fieldOfView = currentState.fov;
			isInit = true;
		}


		public void FixedUpdate(){
			if ( /*mainTarget == null || targetLookAt == null ||*/ currentState == null || lerpState == null ||
			    !isInit ||
			    isFreezed){
				return;
			}
			switch (currentState.cameraMode){
				case CameraMode.FreeDirectional:
					CameraMovement();
					break;
			}
		}

		/// <summary>
		/// 锁定目标
		/// </summary>
		public void SetLockTarget(Transform lockTarget){
			if (this.lockTarget != null && this.lockTarget == lockTarget){
				return;
			}
			isNewTarget = lockTarget != this.lockTarget;
			this.lockTarget = lockTarget;
			lockTargetWeight = 0;
			this.lockTargetSpeed = 1;
		}

		/// <summary>
		/// 锁定目标
		/// </summary>
		/// <param name="lockTarget">Target to look</param>
		/// <param name="heightOffset">Height offset</param>
		/// <param name="lockSpeed">speed to look</param>
		public void SetLockTarget(Transform lockTarget, float heightOffset, float maxRange,
			float lockSpeed = 1, float switchTargetSpeed = 1f){
			if (this.lockTarget != null && this.lockTarget == lockTarget){
				return;
			}
			isNewTarget = lockTarget != this.lockTarget;
			this.lockTarget = lockTarget;
			this.heightOffset = heightOffset;
			this.lockOnMaxRange = maxRange;
			this.lockSwitchTargetSpeed = switchTargetSpeed;
			lockTargetWeight = 0;
			this.lockTargetSpeed = lockSpeed;
		}

		/// <summary>
		/// 取消锁定目标
		/// </summary>
		public virtual void RemoveLockTarget(){
			lockTargetWeight = 0;
			lockTarget = null;
		}


		/// <summary>
		/// 设置新的主目标
		/// </summary>
		/// <param name= "newTarget" ></ param>
		public void SetMainTarget(Transform newTarget){
			mainTarget = newTarget;
			currentTarget = newTarget;
			firstUpdated = true;
			if (!isInit){
				Init();
			}
		}

		/// <summary>
		/// 重置目标
		/// </summary>
		public void ResetTarget(){
			if (currentTarget != mainTarget){
				currentTarget = mainTarget;
				if (!isInit){
					Init();
				}
			}
		}

		/// <summary>
		/// 重置相机角度
		/// </summary>
		public void ResetAngle(){
			if (currentTarget){
				mouseY = currentTarget.eulerAngles.NormalizeAngle().x;
				mouseX = currentTarget.eulerAngles.NormalizeAngle().y;
			} else{
				mouseY = 0;
				mouseX = 0;
			}
		}

		/// <summary>
		/// 设置相机角度
		/// </summary>
		public void SetAngle(Vector2 dir){
			mouseX = dir.x;
			mouseY = dir.y;
		}

		/// <summary>
		/// 设置朝向目标点的角度
		/// </summary>
		/// <param name="targetPoint"></param>
		public void SetAngleByPoint(Vector3 targetPoint, float t){
			Vector3 cameraPos = StateFinalPos;
			Vector3 relativePos = targetPoint - cameraPos;
			Quaternion rotation = Quaternion.LookRotation(relativePos);
			var y = 0f;
			var x = rotation.eulerAngles.y;
			if (rotation.eulerAngles.x < -180){
				y = rotation.eulerAngles.x + 360;
			} else if (rotation.eulerAngles.x > 180){
				y = rotation.eulerAngles.x - 360;
			} else{
				y = rotation.eulerAngles.x;
			}
			mouseX = Mathf.Lerp(mouseX, x, t);
			y -= offsetMouse.y;
			mouseY = Mathf.Lerp(mouseY, y, t);
		}

		/// <summary>
		/// 修改摄像机状态
		/// </summary>
		/// <param name="stateName"></param>
		public virtual void ChangeState(string stateName){
			ChangeState(stateName, true);
		}

		/// <summary>
		/// 修改摄像机状态
		/// </summary>
		/// <param name="stateName"></param>
		/// <param name="Use smoth"></param>
		public void ChangeState(string stateName, bool hasSmooth){
			if (currentState != null && currentState.Name.Equals(stateName) || !isInit && firstStateIsInit){
				if (firstStateIsInit){
					useSmooth = hasSmooth;
				}
				return;
			}
			useSmooth = !firstStateIsInit ? startSmooth : hasSmooth;
			if (CameraStateList == null){
				Debug.LogError("摄像机状态列表缺失");
				return;
			}
			CameraState state = CameraStateList.tpCameraStates.Find(obj => obj.Name.Equals(stateName));
			currentStateName = stateName;
			currentState.cameraMode = state.cameraMode;
			lerpState = state;
			if (!firstStateIsInit){
				currentState.defaultDistance = Vector3.Distance(TargetLookAt.position, transform.position);
				currentState.forward = lerpState.forward;
				currentState.height = state.height;
				currentState.fov = state.fov;
				if (useSmooth){
					StartCoroutine(ResetFirstState());
				} else{
					distance = lerpState.Distance;
					firstStateIsInit = true;
					transformWeight = 1;
				}
			}
			if (currentState != null && !useSmooth){
				currentState.CopyState(state);
				cullingDistance = state.Distance;
			}
			if (currentState == null){
				currentState = new CameraState("Null");
				currentStateName = currentState.Name;
			}
			currentZoom = state.Distance;
			currentState.fixedAngle = new Vector3(mouseX, mouseY);
			// if (!isInit){
			CameraMovement(true);
			// }
		}


		/// <summary>
		/// 触发指定等级的摄像机抖动
		/// </summary>
		public void ShakeCamera(int shakeLevel){
			CameraShakeSettings settings = null;
			foreach (var shakeSetting in shakeSettings){
				if (shakeSetting.level == shakeLevel){
					settings = shakeSetting;
					break;
				}
			}
			if (settings == null){
				Debug.LogError($"缺少该震动级别的配置 {shakeLevel}");
				return;
			}
			shakeDuration = settings.duration;
			shakeMagnitude = settings.magnitude;
			shakeFrequency = settings.frequency;
			shakeTimer = shakeDuration;
		}


		public void FreezeCamera(){
			isFreezed = true;
			if (mainTarget){
				lastLookAtForward = mainTarget.InverseTransformDirection(TargetLookAt.forward);
				lastLookAtPosition = mainTarget.InverseTransformPoint(TargetLookAt.position);
				current_cPos = mainTarget.InverseTransformPoint(current_cPos);
				desired_cPos = mainTarget.InverseTransformPoint(desired_cPos);
			}
		}

		public void UnFreezeCamera(){
			if (mainTarget){
				TargetLookAt.forward = mainTarget.TransformDirection(lastLookAtForward);
				TargetLookAt.position = mainTarget.TransformPoint(lastLookAtPosition);
				current_cPos = mainTarget.TransformPoint(current_cPos);
				desired_cPos = mainTarget.TransformPoint(desired_cPos);
			}
			isFreezed = false;
		}

		/// <summary>
		/// 缩放
		/// </summary>
		/// <param name="scroolValue"></param>
		/// <param name="zoomSpeed"></param>
		public virtual void Zoom(float scroolValue){
			currentZoom -= scroolValue * scrollSpeed;
		}


		/// <summary>
		/// 将摄像机对齐玩家正面，如果有主动的摄像机旋转输入将终止该过程
		/// </summary>
		public void BeginCameraSoftLookAt(float duration){
			softLockDuration = duration;
		}

		public void StopCameraSoftLookAt(){
			softLockDuration = 0;
		}


		public void UpdateAutoCameraLookAt(){
			if (lockTarget) return;
			smoothCameraRotation = Mathf.Lerp(smoothCameraRotation, softLockTargetSmoothRotation,
				6f * Time.fixedDeltaTime);
			var targetSlerpRot = currentTarget.rotation;
			float targetX =
				targetSlerpRot.eulerAngles.NormalizeAngle().y;
			float targetY =
				targetSlerpRot.eulerAngles.NormalizeAngle().x;
			float mouseXDeltaAngle = Math.Abs(Mathf.DeltaAngle(targetX, mouseX));
			float mouseYDeltaAngle = Math.Abs(Mathf.DeltaAngle(targetY, mouseY));
			if (mouseXDeltaAngle > mouseXThreshold){
				mouseX = Mathf.LerpAngle(mouseX, targetX, mouseXCorrectSpeed * Time.fixedDeltaTime);
			}
			if (mouseYDeltaAngle > mouseYThreshold){
				mouseY = Mathf.LerpAngle(mouseY, targetY, mouseYCorrectSpeed * Time.fixedDeltaTime);
			}
		}

		/// <summary>
		/// 摄像机旋转
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void RotateCamera(float x, float y, float scale){
			switch (currentState.cameraMode){
				case CameraMode.FreeDirectional:
					if (!lockTarget){
						// free rotation
						mouseX += x * currentState.XSensitivity * mouseSensitivity * scale;
						mouseY -= y * currentState.YSensitivity * mouseSensitivity * scale;
						movementSpeed.x = x;
						movementSpeed.y = -y;
						cameraIsRotating = movementSpeed.magnitude > 0.5f;
						if (!LockCamera && cameraIsRotating){
							lastRotationTimer = Time.time;
							softLockDuration = 0;
							smoothCameraRotation = initialCameraRotation;
							mouseY = Extensions.ClampAngle(mouseY, lerpState.yMinLimit, lerpState.yMaxLimit);
							mouseX = Extensions.ClampAngle(mouseX, lerpState.xMinLimit, lerpState.xMaxLimit);
						}
					} else{
						smoothCameraRotation = initialCameraRotation;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// 切换左右
		/// </summary>
		/// <param name="value"></param>
		public void SwitchRight(bool value = false){
			switchRight = value ? -1 : 1;
		}

		/// <summary>
		/// 锁定点计算
		/// </summary>
		protected void CalculeLockOnPoint(){
			var collider = lockTarget.GetComponent<Collider>(); // 基于碰撞取中心点
			if (collider == null){
				return;
			}
			var _point = collider.bounds.center /*+ heightOffset * Vector3.up*/;
			Vector3 relativePos = _point - (desired_cPos);
			Quaternion rotation = Quaternion.LookRotation(relativePos);

			//convert angle (360 to 180)
			var y = 0f;
			var x = rotation.eulerAngles.y;
			if (lockTargetWeight < 1f){
				lockTargetWeight += Time.fixedDeltaTime * lockSwitchTargetSpeed;
			}
			if (lockOnVerticalAxis){
				if (rotation.eulerAngles.x < -180){
					y = rotation.eulerAngles.x + 360;
				} else if (rotation.eulerAngles.x > 180){
					y = rotation.eulerAngles.x - 360;
				} else{
					y = rotation.eulerAngles.x;
				}
			} else{
				y = heightOffset - collider.bounds.size.y;
			}
			mouseY = Mathf.LerpAngle(mouseY, Extensions.ClampAngle(y, currentState.yMinLimit, currentState.yMaxLimit),
				lockTargetWeight);
			float targetX = Extensions.ClampAngle(x, currentState.xMinLimit, currentState.xMaxLimit);
			targetX = Mathf.LerpAngle(mouseX, targetX, lockTargetWeight);
			mouseX = Mathf.LerpAngle(mouseX, targetX,
				lockTargetSpeed * Time.deltaTime);
		}

		/// <summary>
		/// 摄像机主移动逻辑，根据当前状态更新摄像机位置、方向、缩放等
		/// </summary>
		/// <param name="forceUpdate">是否强制更新（用于切换状态瞬间）</param>
		protected void CameraMovement(bool forceUpdate = false){
			if (currentTarget == null || targetCamera == null || (!firstStateIsInit && !forceUpdate)){
				return;
			}
			transformWeight = Mathf.Clamp(transformWeight += Time.fixedDeltaTime, 0f, 1f);
			if (useSmooth){
				currentState.Slerp(lerpState, smoothBetweenState * Time.fixedDeltaTime);
			} else{
				currentState.CopyState(lerpState);
			}
			// 自动回正软锁定计时
			softLockDuration -= Time.fixedDeltaTime;
			if (softLockDuration > 0){
				UpdateAutoCameraLookAt();
			}
			UpdateCameraStateData(currentState);
		}


		/// <summary>
		/// 根据摄像机状态数据，计算最终的摄像机位置与方向，并处理遮挡检测、缩放、旋转插值等
		/// </summary>
		protected void UpdateCameraStateData(CameraState cameraState){
			if (cameraState.useZoom){
				currentZoom = Mathf.Clamp(currentZoom, cameraState.minDistance, cameraState.maxDistance);
				distance = useSmooth
					? Mathf.Lerp(distance, currentZoom, lerpState.smooth * Time.fixedDeltaTime)
					: currentZoom;
			} else{
				distance = useSmooth
					? Mathf.Lerp(distance, cameraState.Distance, lerpState.smooth * Time.fixedDeltaTime)
					: cameraState.Distance;
				currentZoom = cameraState.Distance;
			}
			// Fov Lerp
			targetCamera.fieldOfView = Mathf.Lerp(targetCamera.fieldOfView, cameraState.fov, transformWeight);
			cullingDistance = Mathf.Lerp(cullingDistance, currentZoom, smoothBetweenState * Time.fixedDeltaTime);
			currentSwitchRight = Mathf.Lerp(currentSwitchRight, switchRight, smoothSwitchSide * Time.fixedDeltaTime);
			var camDir = (cameraState.forward * TargetLookAt.forward) +
				((cameraState.right * currentSwitchRight) * TargetLookAt.right);
			camDir = camDir.normalized;
			// 获取当前目标的基础位置（主角位置 + 高度偏移）
			var targetPos = new Vector3(currentTarget.position.x, currentTarget.position.y, currentTarget.position.z) +
				currentTarget.transform.up * offSetPlayerPivot;
			// 目标头部 + 状态高度 = 理想位置
			currentTargetPos = targetPos;
			desired_cPos = targetPos + currentTarget.transform.up * cameraState.height;
			Vector3 lerpTargetPos = targetPos + currentTarget.transform.up * currentHeight;
			float smoothDamp = lerpState.smoothDamp; //跟随移动阻尼
			//如果启用阻尼，使用SmoothDamp平滑移动相机位置
			if (smoothDamp > 0){
				current_cPos = firstUpdated
					? lerpTargetPos
					: Vector3.SmoothDamp(current_cPos, lerpTargetPos,
						ref cameraVelocityDamp, smoothDamp * Time.fixedDeltaTime);
			} else current_cPos = lerpTargetPos;
			firstUpdated = false;
			RaycastHit hitInfo;
			// 获取相机裁剪面四个角，用于遮挡检测
			ClipPlanePoints planePoints =
				targetCamera.NearClipPlanePoints(current_cPos + (camDir * (distance)), clipPlaneMargin);
			ClipPlanePoints oldPoints =
				targetCamera.NearClipPlanePoints(desired_cPos + (camDir * currentZoom), clipPlaneMargin);

			//检测上方是否有遮挡
			if (Physics.SphereCast(targetPos, checkHeightRadius, currentTarget.transform.up, out hitInfo,
				    cameraState.cullingHeight + 0.2f, cullingLayer)){
				var t = hitInfo.distance - 0.2f;
				t -= cameraState.height;
				t /= (cameraState.cullingHeight - cameraState.height);
				cullingHeight = Mathf.Lerp(cameraState.height, cameraState.cullingHeight, Mathf.Clamp(t, 0.0f, 1.0f));
			} else{
				cullingHeight = useSmooth
					? Mathf.Lerp(cullingHeight, cameraState.cullingHeight, smoothBetweenState * Time.fixedDeltaTime)
					: cameraState.cullingHeight;
			}

			//检查期望的位置是否未被阻挡
			if (CullingRayCast(desired_cPos, oldPoints, out hitInfo, currentZoom + 0.2f, cullingLayer, Color.blue)){
				var dist = hitInfo.distance;
				if (dist < cameraState.Distance){
					var t = dist;
					t -= cameraState.cullingMinDist;
					t /= (currentZoom - cameraState.cullingMinDist);
					currentHeight = Mathf.Lerp(cullingHeight, cameraState.height, Mathf.Clamp(t, 0.0f, 1.0f));
					current_cPos = targetPos + currentTarget.transform.up * currentHeight;
					lerpTargetPos = targetPos + currentTarget.transform.up * currentHeight;
				}
			} else{
				currentHeight = useSmooth
					? Mathf.Lerp(currentHeight, cameraState.height, smoothBetweenState * Time.fixedDeltaTime)
					: cameraState.height;
			}
			//如果剔除检测比正常距离更小，说明镜头贴墙，需要缩短距离
			if (cullingDistance < distance){
				distance = cullingDistance;
			}

			//检查应用了高度的目标位置是否未被阻挡,最终距离矫正
			if (CullingRayCast(current_cPos, planePoints, out hitInfo, distance, cullingLayer, Color.cyan)){
				distance = Mathf.Clamp(cullingDistance, 0.0f, cameraState.Distance);
			}
			//计算摄像机LookAt方向
			var lookPoint = current_cPos + TargetLookAt.forward * targetCamera.farClipPlane;
			lookPoint += (TargetLookAt.right * Vector3.Dot(camDir * (distance), TargetLookAt.right));
			TargetLookAt.position = current_cPos;
			//旋转计算
			float _mouseY = Mathf.LerpAngle(mouseYStart, mouseY, transformWeight);
			float _mouseX = Mathf.LerpAngle(mouseXStart, mouseX, transformWeight);
			Quaternion newRot = Quaternion.Euler(_mouseY + offsetMouse.y, _mouseX + offsetMouse.x, 0);
			TargetLookAt.rotation = useSmooth
				? Quaternion.Lerp(TargetLookAt.rotation, newRot, lerpState.rotationSmooth * Time.fixedDeltaTime)
				: newRot;

			UpdateShakeOffset();
			// 计算最终相机位置
			Vector3 cameraPosition = Vector3.Lerp(startPosition, current_cPos + camDir * (distance),
				transformWeight);
			cameraPosition += shakeOffset;
			stateFinalPos = lerpTargetPos + (camDir * lerpState.Distance);
			// 实际应用位置
			SelfRigidbody.MovePosition(cameraPosition);
			// 计算摄像机旋转 锁定时通过锁定点计算
			var rotation = Quaternion.LookRotation((lookPoint) - SelfRigidbody.position);
			if (lockTarget){
				CalculeLockOnPoint();
			} else{
				lookTargetAdjust.x = Mathf.LerpAngle(lookTargetAdjust.x, 0, cameraState.smooth * Time.fixedDeltaTime);
				lookTargetAdjust.y = Mathf.LerpAngle(lookTargetAdjust.y, 0, cameraState.smooth * Time.fixedDeltaTime);
				lookTargetAdjust.z = Mathf.LerpAngle(lookTargetAdjust.z, 0, cameraState.smooth * Time.fixedDeltaTime);
			}
			// 计算最终旋转，z轴归0
			var euler = rotation.eulerAngles + lookTargetAdjust;
			euler.z = 0;
			var rot = Quaternion.Euler(euler + cameraState.rotationOffSet);
			Quaternion cameraRot = Quaternion.Lerp(startRotation, rot, transformWeight);
			SelfRigidbody.MoveRotation(cameraRot);
			movementSpeed = Vector2.zero;
		}

		protected bool CullingRayCast(Vector3 from, ClipPlanePoints _to, out RaycastHit hitInfo, float distance,
			LayerMask cullingLayer, Color color){
			bool value = false;
			if (showGizmos){
				Debug.DrawRay(from, _to.LowerLeft - from, color);
				Debug.DrawLine(_to.LowerLeft, _to.LowerRight, color);
				Debug.DrawLine(_to.UpperLeft, _to.UpperRight, color);
				Debug.DrawLine(_to.UpperLeft, _to.LowerLeft, color);
				Debug.DrawLine(_to.UpperRight, _to.LowerRight, color);
				Debug.DrawRay(from, _to.LowerRight - from, color);
				Debug.DrawRay(from, _to.UpperLeft - from, color);
				Debug.DrawRay(from, _to.UpperRight - from, color);
			}
			if (Physics.Raycast(from, _to.LowerLeft - from, out hitInfo, distance, cullingLayer)){
				value = true;
				cullingDistance = hitInfo.distance;
			}
			if (Physics.Raycast(from, _to.LowerRight - from, out hitInfo, distance, cullingLayer)){
				value = true;
				if (cullingDistance > hitInfo.distance){
					cullingDistance = hitInfo.distance;
				}
			}
			if (Physics.Raycast(from, _to.UpperLeft - from, out hitInfo, distance, cullingLayer)){
				value = true;
				if (cullingDistance > hitInfo.distance){
					cullingDistance = hitInfo.distance;
				}
			}
			if (Physics.Raycast(from, _to.UpperRight - from, out hitInfo, distance, cullingLayer)){
				value = true;
				if (cullingDistance > hitInfo.distance){
					cullingDistance = hitInfo.distance;
				}
			}
			return hitInfo.collider && value;
		}

		protected IEnumerator ResetFirstState(){
			yield return new WaitForEndOfFrame();
			firstStateIsInit = true;
		}

		/// <summary>
		/// 更新震动偏移
		/// </summary>
		private void UpdateShakeOffset(){
			if (shakeTimer > 0f){
				shakeTimer -= Time.deltaTime;
				float shakeProgress = 1f - (shakeTimer / shakeDuration);
				float damper = 1f - Mathf.Clamp01(shakeProgress);
				float x = (Mathf.PerlinNoise(Time.time * shakeFrequency, 0f) - 0.5f) * 2f;
				float y = (Mathf.PerlinNoise(0f, Time.time * shakeFrequency) - 0.5f) * 2f;
				float z = (Mathf.PerlinNoise(Time.time * shakeFrequency, Time.time * 0.5f) - 0.5f) * 2f;
				shakeOffset = new Vector3(x, y, z) * shakeMagnitude * damper;
			} else{
				shakeOffset = Vector3.zero;
			}
		}
	}
}
