using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Combat.Runtime{
	[Serializable]
	public class AttributesSetModifier{
		[SerializeField]
		private Guid uid = Guid.NewGuid();
		public List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();


		public void AddModifier(AttributeModifier modifier){
			if (modifier == null){
				Debug.LogWarning("为空的属性修改");
				return;
			}
			attributeModifiers.Add(modifier);
		}

		protected bool Equals(AttributesSetModifier other){
			return uid.Equals(other.uid);
		}

		public void Clear(){
			attributeModifiers.Clear();
		}

		public void TrySetModifierValue(string attributeTag, float value){
			var modifier = attributeModifiers.Find(attributeModifier => attributeModifier.attributeTag == attributeTag);
			if (modifier == null){
				attributeModifiers.Add(new AttributeModifier(attributeTag, value));
				return;
			}
			modifier.value = value;
		}

		public void TryAddModifierValue(string attributeTag, AttributeModifierOperator modifierOperator,
			float value){
			var modifier = attributeModifiers.Find(attributeModifier =>
				attributeModifier.attributeTag == attributeTag &&
				attributeModifier.modifierOperator == modifierOperator);
			if (modifier == null){
				attributeModifiers.Add(new AttributeModifier(attributeTag, modifierOperator, value));
				return;
			}
			modifier.value += value;
		}

		public override string ToString(){
			return $"{nameof(attributeModifiers)}: {attributeModifiers}";
		}
	}
}
