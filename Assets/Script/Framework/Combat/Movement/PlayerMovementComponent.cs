using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework.Combat.Runtime{
	public class PlayerMovementComponent : MovementComponent{
		[Header("Player")]
		public float MoveSpeed = 2.0f;
		public float SprintSpeed = 5.335f;
		[Range(0.0f, 0.3f)]
		public float RotationSmoothTime = 0.12f;

		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Space(10)]
		public float JumpHeight = 1.2f;

		public float Gravity = -15.0f;

		[Space(10)]
		public float JumpTimeout = 0.50f;

		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		public bool Grounded = true;

		public float GroundedOffset = -0.14f;

		public float GroundedRadius = 0.28f;

		public LayerMask GroundLayers;

		// player
		private float speed;
		private float targetRotation = 0.0f;
		private float rotationVelocity;
		private float verticalVelocity;
		private float terminalVelocity = 53.0f;

		// timeout deltatime
		private float jumpTimeoutDelta;
		private float fallTimeoutDelta;

		private bool sprint;

		private Animator animator;
		private CharacterController cc;
		private GameObject mainCamera;
		private Vector3 moveVelocityCache; //外部调用的位移缓存，等待运动组件更新时应用


		private void Awake(){
			cc = GetComponent<CharacterController>();
			mainCamera = Camera.main.gameObject;
		}

		public override void MoveInput(Vector2 input){
			moveInput = input;
		}

		public override void Move(Vector3 offset){
			if(cc.enabled)
				moveVelocityCache += offset;
			else base.Move(offset);
		}

		public override void MoveTo(Vector3 destination){
		}

		public void DoJump(){
			if (!CanMove()) return;
			if (Grounded && jumpTimeoutDelta <= 0.0f){
				//期望高度所需的速度
				verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
			}
		}

		public void SetSprint(bool value){
			sprint = value;
		}

		private void FixedUpdate(){
			// _hasAnimator = TryGetComponent(out _animator);
			if (!cc.enabled) return;
			float deltaTime = Time.fixedDeltaTime * owner.GetTimeScale();
			JumpHandle(deltaTime);
			GravityHandle(deltaTime);
			GroundedCheck(deltaTime);
			MoveHandle(deltaTime);
			ApplyVelocity(deltaTime);
		}


		private void GroundedCheck(float deltaTime){
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
				transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
				QueryTriggerInteraction.Ignore);
		}


		private void ApplyVelocity(float deltaTime){
			Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
			Vector3 finalVelocity = targetDirection.normalized * (speed * deltaTime) +
				new Vector3(0.0f, verticalVelocity, 0.0f) * deltaTime;
			finalVelocity += moveVelocityCache;
			moveVelocityCache = Vector3.zero;
			cc.Move(finalVelocity);
		}

		private void MoveHandle(float deltaTime){
			if (owner.ActionComp && owner.ActionComp.IsPerformingAction){
				speed = 0;
				//执行动作时不响应移动输入
				return;
			}
			float targetSpeed = sprint ? SprintSpeed : MoveSpeed;
			if (moveInput == Vector2.zero) targetSpeed = 0.0f;
			float currentHorizontalSpeed = new Vector3(cc.velocity.x, 0.0f, cc.velocity.z).magnitude;
			float speedOffset = 0.1f;
			float inputMagnitude = moveInput.magnitude;
			if (currentHorizontalSpeed < targetSpeed - speedOffset ||
			    currentHorizontalSpeed > targetSpeed + speedOffset){
				speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
					deltaTime * SpeedChangeRate);
				speed = Mathf.Round(speed * 1000f) / 1000f;
			} else{
				speed = targetSpeed;
			}
			Vector3 inputDirection = new Vector3(moveInput.x, 0.0f, moveInput.y).normalized;
			if (moveInput != Vector2.zero){
				targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
					mainCamera.transform.eulerAngles.y;
				float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity,
					RotationSmoothTime);
				transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
			}
		}

		private void JumpHandle(float deltaTime){
			if (owner.ActionComp && owner.ActionComp.IsPerformingAction){
				return;
			}
			if (Grounded){
				fallTimeoutDelta = FallTimeout;
				if (verticalVelocity < 0.0f){
					verticalVelocity = -2f;
				}
				// jump timeout
				if (jumpTimeoutDelta >= 0.0f){
					jumpTimeoutDelta -= deltaTime;
				}
			} else{
				// reset the jump timeout timer
				jumpTimeoutDelta = JumpTimeout;
				// fall timeout
				if (fallTimeoutDelta >= 0.0f){
					fallTimeoutDelta -= deltaTime;
				}
			}
		}

		private void GravityHandle(float deltaTime){
			if (!gravityEnabled) return;
			if (verticalVelocity < terminalVelocity){
				verticalVelocity += Gravity * deltaTime;
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax){
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected(){
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;
			Gizmos.DrawSphere(
				new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
				GroundedRadius);
		}
	}
}
