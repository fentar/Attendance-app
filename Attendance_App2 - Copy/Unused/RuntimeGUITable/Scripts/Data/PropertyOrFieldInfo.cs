using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace UnityUITable
{

	public class PropertyOrFieldInfo
	{

		PropertyInfo propertyInfo;
		FieldInfo fieldInfo;

		public PropertyOrFieldInfo(MemberInfo memberInfo)
		{
			if (memberInfo == null)
				return;
			if (memberInfo.MemberType == MemberTypes.Field)
				this.fieldInfo = (FieldInfo)memberInfo;
			else if (memberInfo.MemberType == MemberTypes.Property)
				this.propertyInfo = (PropertyInfo)memberInfo;
		}

		public PropertyOrFieldInfo(PropertyInfo propertyInfo, FieldInfo fieldInfo)
		{
			this.propertyInfo = propertyInfo;
			this.fieldInfo = fieldInfo;
		}

		public void SetValue(object obj, object value)
		{
			if (propertyInfo != null)
			{
				if (propertyInfo.GetSetMethod(true) == null)
					Debug.LogErrorFormat("No Set Method found in {0}. Cell should not be interactable.", propertyInfo.Name);
				else
					propertyInfo.SetValue(obj, value, null);
			}
			else if (fieldInfo != null)
				fieldInfo.SetValue(obj, value);
		}

		public object GetValue(object obj)
		{
			if (propertyInfo != null)
				return propertyInfo.GetValue(obj, null);
			else if (fieldInfo != null)
				return fieldInfo.GetValue(obj);
			return null;
		}

		public Type Type
		{
			get
			{
				if (propertyInfo != null)
					return propertyInfo.PropertyType;
				else if (fieldInfo != null)
					return fieldInfo.FieldType;
				return null;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return propertyInfo == null && fieldInfo == null;
			}
		}

		public bool IsSettable
		{
			get
			{
				return (fieldInfo != null) || propertyInfo.GetSetMethod(true) != null;
			}
		}

		public static Type GetPropertyOrFieldType(MemberInfo member)
		{
			if (member.MemberType == MemberTypes.Property)
				return ((PropertyInfo)member).PropertyType;
			if (member.MemberType == MemberTypes.Field)
				return ((FieldInfo)member).FieldType;
			return null;
		}

	}

}
