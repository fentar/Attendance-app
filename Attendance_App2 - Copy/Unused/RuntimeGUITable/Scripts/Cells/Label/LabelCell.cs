using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace UnityUITable
{

	public class LabelCell : StyleableTableCell<LabelCellStyle>
	{

		public override bool IsCompatibleWithMember(MemberInfo member)
		{
			return member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field;
		}

		public override int GetPriority(MemberInfo member)
		{
			if (member.MemberType == MemberTypes.Field && ((FieldInfo)member).FieldType == typeof(string)
			|| member.MemberType == MemberTypes.Property && ((PropertyInfo)member).PropertyType == typeof(string))
				return 10;
			return base.GetPriority(member);
		}

		public Text label;

		public override void UpdateContent()
		{
			if (property == null || property.IsEmpty)
				return;
			object o = property.GetValue(obj);
			if (o == null)
				label.text = "null";
			else if (Table.IsCollection(o.GetType()))
				label.text = string.Join(", ", (o as IEnumerable).OfType<object>().Select(obj => obj.ToString()).ToArray());
			else
				label.text = o.ToString();
		}

	}

}
