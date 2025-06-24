using System;
using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 主组件，控制游戏初始化
	/// </summary>
	public class MainComponent : FrameworkComponent{
		public CameraController cameraController;
		public PlayerController playerController;
		public float frameTime = 0.0166667f;
		public bool unlockCursorOnStart = false;
		public bool showCursorOnStart = false;


		private Action initCallback = delegate{ };
		private bool initialized;

		private void Start(){
			ShowCursor(showCursorOnStart);
			LockCursor(unlockCursorOnStart);
		}

		public void ShowCursor(bool value){
			Cursor.visible = value;
		}

		public void LockCursor(bool value){
			if (!value){
				Cursor.lockState = CursorLockMode.Locked;
			} else{
				Cursor.lockState = CursorLockMode.None;
			}
		}

		/// <summary>
		/// 暂停游戏。
		/// </summary>
		public void PauseGame(){
			GameEntry.Time.Pause();
		}

		/// <summary>
		/// 恢复游戏。
		/// </summary>
		public void ResumeGame(){
			GameEntry.Time.Continue();
		}


		/// <summary>
		/// todo 关闭框架及所有组件，Demo暂时用不到
		/// </summary>
		public void Shutdown(){
			Destroy(gameObject);
		}
	}
}
