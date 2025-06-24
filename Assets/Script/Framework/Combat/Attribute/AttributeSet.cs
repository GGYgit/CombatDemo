using System.Collections.Generic;
using UnityEngine;

namespace Framework.Combat.Runtime{
	/// <summary>
	/// 属性集
	/// </summary>
	[CreateAssetMenu(menuName = "Config/AttributeSet")]
	public class AttributeSet:ScriptableObject{
		public List<AttributeValueConfig> attributes;


	}
}
