using System.Collections.Generic;
using UnityEngine;


namespace Framework.Combat.Runtime{
	/// <summary>
	/// 属性定义
	/// </summary>
	[CreateAssetMenu(menuName = "Config/Attribute")]
	public class AttributeScriptableObject : ScriptableObject{
		/// <summary>
		/// 属性名
		/// </summary>
		public string Tag;


		public virtual AttributeValue CalculateInitialValue(AttributeValue attributeValue,
			List<AttributeValue> otherAttributeValues){
			return attributeValue;
		}

		public virtual AttributeValue CalculateCurrentAttributeValue(AttributeValue attributeValue,
			Dictionary<string,AttributeValue> otherAttributeValues){
			float lastValue = attributeValue.CurrentValue;
			attributeValue.CurrentValue = (attributeValue.BaseValue + attributeValue.Modifier.Add +
					attributeValue.instantValueChanges) *
				(attributeValue.Modifier.Multiply + 1);
			if (attributeValue.Modifier.Override != 0){
				attributeValue.CurrentValue = attributeValue.Modifier.Override;
			}
			return attributeValue;
		}
	}
}
