using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Combat.Runtime{
	public class AttributeComponent : CharacterComponent{
		[SerializeField]
		private AttributeSet attributeSetConfig;

		[Header("Debug")]
		[ShowInInspector]
		private List<AttributesSetModifier> activeModifiers; //激活的属性集修改器
		[ShowInInspector]
		private Dictionary<string, AttributeValue> attributeValueDic;
		private bool attributeDictStale = true;

		protected override void OnInit(BaseCharacter character){
			base.OnInit(character);
			activeModifiers = new List<AttributesSetModifier>();
			InitialiseAttributeValues();
		}


		/// <summary>
		/// 获取属性当前值
		/// </summary>
		/// <param name="tag"></param>
		public float GetCurrentValue(string tag){
			if (attributeValueDic.TryGetValue(tag, out AttributeValue value)){
				return value.CurrentValue;
			}
			return 0;
		}

		/// <summary>
		/// 设置属性基础值
		/// </summary>
		public void SetBaseValue(string tag, float value){
			if (attributeValueDic.TryGetValue(tag, out AttributeValue attributeValue)){
				attributeValue.BaseValue = value;
				UpdateAttributeCurrentValues();
			}
		}

		/// <summary>
		/// 加当前值
		/// </summary>
		public void AddCurrentValue(string tag, float value){
			if (attributeValueDic.TryGetValue(tag, out AttributeValue attributeValue)){
				attributeValue.instantValueChanges += value;
				UpdateAttributeCurrentValues();
			}
		}

		/// <summary>
		/// 重置当前值
		/// </summary>
		public void ResetCurrentValue(string tag){
			if (attributeValueDic.TryGetValue(tag, out AttributeValue attributeValue)){
				attributeValue.instantValueChanges = 0;
				UpdateAttributeCurrentValues();
			}
		}

		/// <summary>
		/// 添加属性集修改
		/// </summary>
		public void AddAttributeSetModifier(AttributesSetModifier attributesSetModifier){
			if (activeModifiers.Contains(attributesSetModifier)){
				Debug.LogWarning("AttributeSystem - 不允许重复添加属性集修改");
				return;
			}
			activeModifiers.Add(attributesSetModifier);
			if (attributesSetModifier.attributeModifiers.Count <= 0) return;
			CalculateModifiers();
		}

		/// <summary>
		/// 移除属性集修改
		/// </summary>
		public void RemoveAttributeSetModifier(AttributesSetModifier attributesSetModifier){
			if (!activeModifiers.Remove(attributesSetModifier)){
				Debug.LogWarning("AttributeSystem - 无法删除不存在的属性集修改");
				return;
			}
			CalculateModifiers();
		}

		private void InitialiseAttributeValues(){
			var attributes = attributeSetConfig.attributes;
			attributeValueDic = new Dictionary<string, AttributeValue>(attributes.Count);
			for (var i = 0; i < attributes.Count; i++){
				if (string.IsNullOrEmpty(attributes[i].attribute.Tag)) continue;
				AttributeValue attributeValue = new AttributeValue(){
					Attribute = attributes[i].attribute,
					Modifier = new AttributeModifierSpec(){
						Add = 0f,
						Multiply = 0f,
						Override = 0f
					},
					BaseValue = attributes[i].defaultValue
				};
				attributeValueDic.Add(attributes[i].attribute.Tag, attributeValue);
			}
			UpdateAttributeCurrentValues();
		}

		/// <summary>
		/// 重置并计算属性数据
		/// </summary>
		private void CalculateModifiers(){
			//重置属性修改器
			foreach (var attributeValue in attributeValueDic){
				attributeValue.Value.Modifier = default;
			}
			//重新合并
			for (int i = 0; i < activeModifiers.Count; i++){
				var attributeSetModifiers = activeModifiers[i].attributeModifiers;
				for (int j = 0; j < attributeSetModifiers.Count; j++){
					AttributeModifier modifier = attributeSetModifiers[j];
					AttributeModifierSpec attributeModifierSpec = modifier.CreateSpec();
					CombieAttributeModifiers(modifier.attributeTag, attributeModifierSpec);
				}
			}
			UpdateAttributeCurrentValues();
		}

		/// <summary>
		/// 合并属性修改
		/// </summary>
		private void CombieAttributeModifiers(string tag, AttributeModifierSpec modifier){
			if (attributeValueDic.TryGetValue(tag, out var attributeValue)){
				attributeValue.Modifier = attributeValue.Modifier.Combine(modifier);
			}
			// 无匹配属性
		}

		/// <summary>
		/// 更新属性
		/// CurrentValue将会在此更新
		/// </summary>
		private void UpdateAttributeCurrentValues(){
			foreach (var attributeValuePair in attributeValueDic){
				AttributeValue currentAttribute = attributeValuePair.Value;
				currentAttribute.Attribute.CalculateCurrentAttributeValue(currentAttribute, attributeValueDic); //计算当前属性
			}
		}
	}
}
