using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Framework.Combat.Runtime{
	[CustomEditor(typeof(CameraController), true)]
	[CanEditMultipleObjects]
	public class CameraEditor : UnityEditor.Editor{
		GUISkin skin;
		CameraController tpCameraController;
		bool hasPointCopy;
		Vector3 pointCopy;
		int indexSelected;


		void OnEnable(){
			indexSelected = 0;
			tpCameraController = (CameraController) target;
		}

		public override void OnInspectorGUI(){

			tpCameraController = (CameraController) target;
			EditorGUILayout.Space();
			GUILayout.BeginVertical("Third Person Camera", "window");
			// GUILayout.Label(m_Logo, GUILayout.MaxHeight(25));
			GUILayout.Label("", GUILayout.MaxHeight(25));
			GUILayout.Space(5);
			base.OnInspectorGUI();
			if (GUILayout.Button(new GUIContent("Open CameraEventList"))){
				EditorUtility.OpenPropertyEditor(tpCameraController.cameraEventListData);
			}
			GUILayout.EndVertical();
			GUILayout.BeginVertical("Camera States", "window");
			GUILayout.Label("", GUILayout.MaxHeight(25));
			// GUILayout.Label(m_Logo, GUILayout.MaxHeight(25));
			GUILayout.Space(5);
			tpCameraController.CameraStateList = (CameraListData) EditorGUILayout.ObjectField("CameraState List",
				tpCameraController.CameraStateList, typeof(CameraListData), false);
			if (tpCameraController.CameraStateList == null){
				GUILayout.EndVertical();
				return;
			}
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(new GUIContent("New CameraState"))){
				if (tpCameraController.CameraStateList.tpCameraStates == null)
					tpCameraController.CameraStateList.tpCameraStates = new List<CameraState>();
				tpCameraController.CameraStateList.tpCameraStates.Add(
					new CameraState("New State" + tpCameraController.CameraStateList.tpCameraStates.Count));
				tpCameraController.indexList = tpCameraController.CameraStateList.tpCameraStates.Count - 1;
			}
			if (GUILayout.Button(new GUIContent("Delete State")) && tpCameraController.CameraStateList.tpCameraStates.Count > 1 &&
			    tpCameraController.indexList != 0){
				tpCameraController.CameraStateList.tpCameraStates.RemoveAt(tpCameraController.indexList);
				if (tpCameraController.indexList - 1 >= 0)
					tpCameraController.indexList--;
			}
			GUILayout.EndHorizontal();
			if (tpCameraController.CameraStateList.tpCameraStates.Count > 0){
				if (tpCameraController.indexList > tpCameraController.CameraStateList.tpCameraStates.Count - 1) tpCameraController.indexList = 0;
				tpCameraController.indexList = EditorGUILayout.Popup("State", tpCameraController.indexList,
					getListName(tpCameraController.CameraStateList.tpCameraStates));
				StateData(tpCameraController.CameraStateList.tpCameraStates[tpCameraController.indexList]);
			}
			GUILayout.EndVertical();

			EditorGUILayout.Space();
			if (GUI.changed){
				EditorUtility.SetDirty(tpCameraController);
				EditorUtility.SetDirty(tpCameraController.CameraStateList);
			}
		}


		void StateData(CameraState camState){
			EditorGUILayout.Space();
			DrawEnumField("Camera Mode", ref camState.cameraMode);
			DrawTextField("State Name", ref camState.Name);
			if (CheckName(camState.Name, tpCameraController.indexList)){
				EditorGUILayout.HelpBox("This name already exist, choose another one", MessageType.Error);
			}
			switch (camState.cameraMode){
				case CameraMode.FreeDirectional:
					FreeDirectionalMode(camState);
					break;
			}
		}


		void FreeDirectionalMode(CameraState camState){
			DrawSliderField("Forward", ref camState.forward, -1f, 1f);
			DrawSliderField("Right", ref camState.right, -3f, 3f);
			DrawFloatField("Distance", ref camState.defaultDistance);
			DrawToogleField("Use Zoom", ref camState.useZoom);
			if (camState.useZoom){
				DrawFloatField("Max Distance", ref camState.maxDistance);
				DrawFloatField("Min Distance", ref camState.minDistance);
			}
			DrawFloatField("Height", ref camState.height);
			DrawSliderField("Field of View", ref camState.fov, 1, 179);
			DrawFloatField("State Smooth", ref camState.smooth);
			DrawFloatField("Smooth Damp", ref camState.smoothDamp);
			DrawFloatField("Rotation Smooth", ref camState.rotationSmooth);
			DrawFloatField("Culling Height", ref camState.cullingHeight);
			DrawVector3Field("Rotation OffSet", ref camState.rotationOffSet);
			DrawFloatField("Rotation Sensitivity X", ref camState.xDefaultSensitivity);
			DrawFloatField("Rotation Sensitivity Y", ref camState.yDefaultSensitivity);
			MinMaxSliderField("Limit Angle X", ref camState.xMinLimit, ref camState.xMaxLimit, -360, 360);
			MinMaxSliderField("Limit Angle Y", ref camState.yMinLimit, ref camState.yMaxLimit, -180, 180);
		}

		void FixedAngleMode(CameraState camState){
			DrawFloatField("Distance", ref camState.defaultDistance);
			DrawToogleField("Use Zoom", ref camState.useZoom);
			if (camState.useZoom){
				DrawFloatField("Max Distance", ref camState.maxDistance);
				DrawFloatField("Min Distance", ref camState.minDistance);
			}
			DrawFloatField("Height", ref camState.height);
			DrawSliderField("Field of View", ref camState.fov, 1, 179);
			DrawFloatField("Smooth Follow", ref camState.smooth);
			DrawFloatField("Culling Height", ref camState.cullingHeight);
			DrawSliderField("Right", ref camState.right, -3f, 3f);
			DrawSliderField("Angle X", ref camState.fixedAngle.x, -360, 360);
			DrawSliderField("Angle Y", ref camState.fixedAngle.y, -360, 360);
		}



		bool CheckName(string Name, int _index){
			foreach (CameraState state in tpCameraController.CameraStateList.tpCameraStates)
				if (state.Name.Equals(Name) && tpCameraController.CameraStateList.tpCameraStates.IndexOf(state) != _index)
					return true;
			return false;
		}

#region Camera State Drawers with undo
		void DrawEnumField<T>(string name, ref T value) where T : System.Enum{
			T _value = value;
			EditorGUI.BeginChangeCheck();
			_value = (T) EditorGUILayout.EnumPopup(name, _value);
			if (EditorGUI.EndChangeCheck()){
				Undo.RecordObject(tpCameraController.CameraStateList, "ChangeCameraState");
				value = _value;
			}
		}

		void DrawTextField(string name, ref string value){
			string _value = value;
			EditorGUI.BeginChangeCheck();
			_value = EditorGUILayout.TextField(name, _value);
			if (EditorGUI.EndChangeCheck()){
				Undo.RecordObject(tpCameraController.CameraStateList, "ChangeCameraState");
				value = _value;
			}
		}

		void DrawVector3Field(string name, ref Vector3 value){
			Vector3 _value = value;
			EditorGUI.BeginChangeCheck();
			_value = EditorGUILayout.Vector3Field("Rotation OffSet", _value);
			if (EditorGUI.EndChangeCheck()){
				Undo.RecordObject(tpCameraController.CameraStateList, "ChangeCameraState");
				value = _value;
			}
		}

		void DrawSliderField(string name, ref float value, float min, float max){
			float _value = value;
			EditorGUI.BeginChangeCheck();
			_value = EditorGUILayout.Slider(name, _value, min, max);
			if (EditorGUI.EndChangeCheck()){
				Undo.RecordObject(tpCameraController.CameraStateList, "ChangeCameraState");
				value = _value;
			}
		}

		void DrawFloatField(string name, ref float value){
			float _value = value;
			EditorGUI.BeginChangeCheck();
			_value = EditorGUILayout.FloatField(name, _value);
			if (EditorGUI.EndChangeCheck()){
				Undo.RecordObject(tpCameraController.CameraStateList, "ChangeCameraState");
				value = _value;
			}
		}

		void DrawToogleField(string name, ref bool value){
			bool _value = value;
			EditorGUI.BeginChangeCheck();
			_value = EditorGUILayout.Toggle(name, _value);
			if (EditorGUI.EndChangeCheck()){
				Undo.RecordObject(tpCameraController.CameraStateList, "ChangeCameraState");
				value = _value;
			}
		}

		void MinMaxSliderField(string name, ref float minVal, ref float maxVal, float minLimit, float maxLimit){
			float _minVal = minVal;
			float _maxVal = maxVal;
			GUILayout.BeginVertical();
			GUILayout.Label(name);
			GUILayout.BeginHorizontal("box");
			EditorGUI.BeginChangeCheck();
			_minVal = EditorGUILayout.FloatField(_minVal, GUILayout.MaxWidth(60));
			EditorGUILayout.MinMaxSlider(ref _minVal, ref _maxVal, minLimit, maxLimit);
			_maxVal = EditorGUILayout.FloatField(_maxVal, GUILayout.MaxWidth(60));
			if (EditorGUI.EndChangeCheck()){
				Undo.RecordObject(tpCameraController.CameraStateList, "ChangeCameraState");
				minVal = _minVal;
				maxVal = _maxVal;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}
#endregion


		private string[] getListName(List<CameraState> list){
			string[] names = new string[list.Count];
			for (int i = 0; i < list.Count; i++){
				names[i] = list[i].Name;
			}
			return names;
		}
	}
}
