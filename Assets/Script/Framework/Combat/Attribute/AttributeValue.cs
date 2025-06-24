using System;

namespace Framework.Combat.Runtime{
	[Serializable]
	public class AttributeValue{
		public AttributeScriptableObject Attribute;
		public float BaseValue;//不受任何加成的基础值
		public float CurrentValue;//最终计算后的当前值
		public float instantValueChanges;//瞬时的当前值修改，AddCurrentValue会对该值做修改
		public AttributeModifierSpec Modifier;


		public void Copy(AttributeValue target){
			Attribute = target.Attribute;
			BaseValue = target.BaseValue;
			CurrentValue = target.CurrentValue;
			Modifier = target.Modifier;
		}

	}



	[Serializable]
	public struct AttributeModifierSpec{
		public float Add;
		public float Multiply;
		public float Override;

		public AttributeModifierSpec Combine(AttributeModifierSpec other){
			other.Add += Add;
			other.Multiply += Multiply;
			other.Override = Override;
			return other;
		}
	}

	[Serializable]
	public struct AttributeValueConfig{
		public AttributeScriptableObject attribute;
		public float defaultValue;
	}

}
