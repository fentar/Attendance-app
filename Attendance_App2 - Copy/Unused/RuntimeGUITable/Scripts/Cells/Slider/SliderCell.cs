using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityUITable
{

	public class SliderCell : InteractableCell<SliderCellStyle>
	{

		public override bool IsCompatibleWithMember(MemberInfo member)
		{
			return IsOfCompatibleType(member, typeof(float), typeof(int));
		}

		public override int GetPriority(MemberInfo member)
		{
			return -10;
		}

		public Slider slider;

		public override void UpdateContent()
		{
			if (property == null || property.IsEmpty)
				return;
			if (property.Type == typeof(float))
				slider.value = (float)property.GetValue(obj);
			else if (property.Type == typeof(int))
				slider.value = (float)((int)property.GetValue(obj));
		}

		public void OnValueChanged()
		{
			if (!interactable)
				return;
			if (property.Type == typeof(float))
				SetValue(obj, slider.value);
			else if (property.Type == typeof(int))
				SetValue(obj, (int)slider.value);
		}

		protected override void ApplyCustomStyle(SliderCellStyle style)
		{
			slider.minValue = style.minValue;
			slider.maxValue = style.maxValue;
			if (slider.handleRect.GetComponent<Image>() != null)
			{
				slider.handleRect.GetComponent<Image>().color = style.handleColor;
				slider.handleRect.GetComponent<Image>().sprite = style.handleSprite;
			}
			if (slider.transform.Find("Background") != null && slider.transform.Find("Background").GetComponent<Image>())
			{
				slider.transform.Find("Background").GetComponent<Image>().color = style.bgColor;
				slider.transform.Find("Background").GetComponent<Image>().sprite = style.bgSprite;
			}
			if (slider.fillRect != null && slider.fillRect.GetComponent<Image>() != null)
			{
				slider.fillRect.GetComponent<Image>().color = style.fillColor;
				slider.fillRect.GetComponent<Image>().sprite = style.bgSprite;
			}
		}

		protected override void SetDefaultCustomSettings(SliderCellStyle style)
		{
#if UNITY_EDITOR
			style.minValue = slider.minValue;
			style.maxValue = slider.maxValue;
			if (slider.handleRect.GetComponent<Image>() != null)
			{
				style.handleColor = slider.handleRect.GetComponent<Image>().color;
				style.handleSprite = slider.handleRect.GetComponent<Image>().sprite;
			}
			else
			{
				style.handleColor = Color.white;
				style.handleSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
			}
			if (slider.transform.Find("Background") != null && slider.transform.Find("Background").GetComponent<Image>())
			{
				style.bgColor = slider.transform.Find("Background").GetComponent<Image>().color;
				style.bgSprite = slider.transform.Find("Background").GetComponent<Image>().sprite;
			}
			else
			{
				style.bgColor = Color.gray;
				style.bgSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
			}
			if (slider.fillRect != null && slider.fillRect.GetComponent<Image>() != null)
				style.fillColor = slider.fillRect.GetComponent<Image>().color;
			else
				style.fillColor = Color.white;
#endif
		}
	}

}
