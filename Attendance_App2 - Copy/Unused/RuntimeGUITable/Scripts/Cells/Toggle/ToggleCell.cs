using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace UnityUITable
{

	public class ToggleCell : InteractableCell<ToggleCellStyle>
	{

		public override bool IsCompatibleWithMember(MemberInfo member)
		{
			return IsOfCompatibleType(member, typeof(bool));
		}

		public override int GetPriority(MemberInfo member)
		{
			return 10;
		}

		public Toggle toggle;

		public override void UpdateContent()
		{
			if (property == null || property.IsEmpty)
				return;
			toggle.isOn = (bool)property.GetValue(obj);
		}

		public void OnValueChanged()
		{
			// Avoid executing on the prefab when called by ForceUpdateCanvases
			if(gameObject.scene.name == null)
				return;
			SetValue(obj, toggle.isOn);
		}
	}

}
