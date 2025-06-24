using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Framework.Combat.Runtime{
	[System.Serializable]
	[CreateAssetMenu(menuName = "CombatFramework/CameraEvent List")]
	public class CameraEventListData : ScriptableObject{
		[SerializeField] public string Name;
		[SerializeField, TableList(AlwaysExpanded = true)]
		private List<CameraEventData> cameraEventDatas;

		private Dictionary<string, CameraEventData> cameraEventDic;
		public Dictionary<string, CameraEventData> CameraEventDic => cameraEventDic;

		public CameraEventListData(){
			cameraEventDatas = new List<CameraEventData>();
			cameraEventDatas.Add(new CameraEventData("Default"));
		}

		public bool Get(string eventName, out CameraEventData eventData){
			return cameraEventDic.TryGetValue(eventName, out eventData);
		}

		public void Init(){
			cameraEventDic = new Dictionary<string, CameraEventData>();
			for (int i = 0; i < cameraEventDatas.Count; i++){
				cameraEventDic.Add(cameraEventDatas[i].Name, cameraEventDatas[i]);
			}
		}
	}
}
