using UnityEngine;
using UnityEngine.InputSystem;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 玩家控制器
	/// </summary>
	public sealed class PlayerController : BaseController{
		private PlayerCharacter playerCharacter;
		private PlayerInputAction playerInputAction;
		private PlayerInputAction.GamePlayActions gamePlayActionSet;
		private Vector2 rotateCameraInput;
		private CameraController cameraController;
		private PlayerMovementComponent movementComp;
		private WeaponComponent weaponComp;

		private void Update(){
			if (playerCharacter == null || !playerCharacter.IsEntityAlive()) return;
			MovementInput();
			ActionInput();
		}

		private void LateUpdate(){
			if (playerCharacter == null || !playerCharacter.IsEntityAlive()) return;
			CameraRotate();
		}

		protected override void OnPossess(BaseCharacter character){
			base.OnPossess(character);
			playerCharacter = character as PlayerCharacter;
			movementComp = playerCharacter.GetCharComponent<PlayerMovementComponent>();
			cameraController = GameEntry.Main.cameraController;
			weaponComp = playerCharacter.GetCharComponent<WeaponComponent>();
			RegisterInputComponent();
		}

		protected override void OnUnPossess(){
			base.OnUnPossess();
			UnRegisterInputComponent();
		}


#region Input Method
		private void RegisterInputComponent(){
			playerInputAction ??= new PlayerInputAction();
			gamePlayActionSet = playerInputAction.GamePlay;
			gamePlayActionSet.Enable();
			playerCharacter.SetupPlayerInputComponent(gamePlayActionSet);
			gamePlayActionSet.RotateCamera.started += CameraInputUpdate;
			gamePlayActionSet.RotateCamera.performed += CameraInputUpdate;
			gamePlayActionSet.RotateCamera.canceled += CameraInputUpdate;
		}


		private void UnRegisterInputComponent(){
			if (playerInputAction == null) return;
			gamePlayActionSet.RotateCamera.started -= CameraInputUpdate;
			gamePlayActionSet.RotateCamera.performed -= CameraInputUpdate;
			gamePlayActionSet.RotateCamera.canceled -= CameraInputUpdate;
			playerInputAction = null;
			gamePlayActionSet = default;
		}


		private void CameraInputUpdate(InputAction.CallbackContext context){
			rotateCameraInput = context.ReadValue<Vector2>();
		}

		private void CameraRotate(){
			var rotateCamera = rotateCameraInput;
			var moveX = rotateCamera.x;
			var moveY = rotateCamera.y;
			float scale = 1;
			cameraController.RotateCamera(moveX, moveY, scale);
		}

		private void MovementInput(){
			Vector2 moveInput = gamePlayActionSet.Move.ReadValue<Vector2>();
			movementComp.MoveInput(moveInput);
			bool sprintButton = gamePlayActionSet.Sprint.IsPressed();
			movementComp.SetSprint(sprintButton);
			bool jumpButton = gamePlayActionSet.Jump.WasPressedThisFrame();
			if (jumpButton)
				movementComp.DoJump();
		}

		private void ActionInput(){
			bool attackButton = gamePlayActionSet.Attack.WasPressedThisFrame();
			bool dodgeButton = gamePlayActionSet.Dodge.WasPressedThisFrame();
			if (attackButton){
				if (movementComp.Grounded)
					playerCharacter.ActionComp.ReceiveInput(ActionConstantTag.Attack, ActionPriority.Low);
				else{
					playerCharacter.ActionComp.ReceiveInput(ActionConstantTag.DashToTarget, ActionPriority.Low);
				}
			}
			if (dodgeButton && movementComp.Grounded){
				playerCharacter.ActionComp.ReceiveInput(ActionConstantTag.Dodge, ActionPriority.High);
			}
		}
#endregion
	}
}
