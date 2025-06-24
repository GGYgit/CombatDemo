using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Framework.Combat.Runtime{
	[System.Serializable]
	[CreateAssetMenu(menuName = "CombatFramework/CameraState List")]
	public class CameraListData : ScriptableObject{
		[SerializeField] public string Name;
		[SerializeField] public List<CameraState> tpCameraStates;

		public CameraListData(){
			tpCameraStates = new List<CameraState>();
			tpCameraStates.Add(new CameraState("Default"));
		}
	}
}
