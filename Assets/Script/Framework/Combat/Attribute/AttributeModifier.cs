using System;

namespace Framework.Combat.Runtime{
	[Serializable]
	public class AttributeModifier{
		public string attributeTag;
		public AttributeModifierOperator modifierOperator;
		public float value;

		public AttributeModifier(){
		}

		public AttributeModifier(string attributeTag, float value){
			this.attributeTag = attributeTag;
			this.modifierOperator = AttributeModifierOperator.Add;
			this.value = value;
		}

		public AttributeModifier(string attributeTag, AttributeModifierOperator modifierOperator, float value){
			this.attributeTag = attributeTag;
			this.modifierOperator = modifierOperator;
			this.value = value;
		}

		/// <summary>
		/// 创建属性修改实例
		/// </summary>
		public AttributeModifierSpec CreateSpec(){
			AttributeModifierSpec modifierSpec = default;
			switch (modifierOperator){
				case AttributeModifierOperator.Add:
					modifierSpec.Add = value;
					break;
				case AttributeModifierOperator.Multiply:
					modifierSpec.Multiply = value;
					break;
				case AttributeModifierOperator.Override:
					modifierSpec.Override = value;
					break;
			}
			return modifierSpec;
		}

		public override string ToString(){
			return
				$"{nameof(attributeTag)}: {attributeTag}, {nameof(modifierOperator)}: {modifierOperator}, {nameof(value)}: {value}";
		}
	}


	public enum AttributeModifierOperator{
		Add,
		Multiply,
		Override
	}
}
