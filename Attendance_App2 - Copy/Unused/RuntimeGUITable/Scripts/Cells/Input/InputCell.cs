using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

namespace UnityUITable
{

	public class InputCell : InteractableCell<InputCellStyle>
	{

		public override bool IsCompatibleWithMember(MemberInfo member)
		{
			Type type = PropertyOrFieldInfo.GetPropertyOrFieldType(member);
			return IsIntegerType(type) || IsDecimalType(type) || IsStringType(type);
		}

		public override int GetPriority(MemberInfo member)
		{
			return -10;
		}

		public InputField inputField;

		public override void UpdateContent()
		{
			if (property == null || property.IsEmpty)
				return;
			object o = property.GetValue(obj);
			if (o != null)
				inputField.text = o.ToString();
			if (IsDecimalType(property.Type))
				inputField.contentType = InputField.ContentType.DecimalNumber;
			if (IsIntegerType(property.Type))
				inputField.contentType = InputField.ContentType.IntegerNumber;
			if (IsStringType(property.Type))
				inputField.contentType = InputField.ContentType.Standard;
		}

		public void OnValueChanged()
		{
			SetValue(obj, System.Convert.ChangeType(inputField.text, property.Type));
		}

		static bool IsIntegerType(Type type)
		{
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
					return true;
				default:
					return false;
			}
		}
		static bool IsDecimalType(Type type)
		{
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
				default:
					return false;
			}
		}
		static bool IsStringType(Type type)
		{
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.String:
				case TypeCode.Char:
					return true;
				default:
					return false;
			}
		}

	}

}
