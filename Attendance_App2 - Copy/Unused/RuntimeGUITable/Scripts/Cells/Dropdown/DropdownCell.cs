using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

namespace UnityUITable
{

	public class DropdownCell : InteractableCell<DropdownCellStyle>
	{

		public override bool IsCompatibleWithMember(MemberInfo member)
		{
			Type propType = PropertyOrFieldInfo.GetPropertyOrFieldType(member);
			return propType != null && propType.IsEnum;
		}

		public override int GetPriority(MemberInfo member)
		{
			return 10;
		}

		public Dropdown dropdown;

		public override void UpdateContent()
		{
			if (property == null || property.IsEmpty)
				return;
			dropdown.value = (int)property.GetValue(obj);
			dropdown.options = Enum.GetNames(property.Type).Select(n => new Dropdown.OptionData(n)).ToList();
		}

		public void OnValueChanged()
		{
			SetValue(obj, dropdown.value);
		}

	}

}
