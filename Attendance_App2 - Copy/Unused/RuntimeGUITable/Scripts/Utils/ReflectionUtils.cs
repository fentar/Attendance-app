using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

namespace UnityUITable
{

	public static class ReflectionUtils
	{

		public static bool IsGenericTypeOf(this Type t, Type genericDefinition)
		{
			Type[] parameters = null;
			return IsGenericTypeOf(t, genericDefinition, out parameters);
		}

		public static bool IsGenericTypeOf(this Type t, Type genericDefinition, out Type[] genericParameters)
		{
			genericParameters = new Type[] { };
			if (!genericDefinition.IsGenericType)
			{
				return false;
			}

			var isMatch = t.IsGenericType && t.GetGenericTypeDefinition() == genericDefinition.GetGenericTypeDefinition();
			if (!isMatch && t.BaseType != null)
			{
				isMatch = IsGenericTypeOf(t.BaseType, genericDefinition, out genericParameters);
			}
			if (!isMatch && genericDefinition.IsInterface && t.GetInterfaces().Any())
			{
				foreach (var i in t.GetInterfaces())
				{
					if (i.IsGenericTypeOf(genericDefinition, out genericParameters))
					{
						isMatch = true;
						break;
					}
				}
			}

			if (isMatch && !genericParameters.Any())
			{
				genericParameters = t.GetGenericArguments();
			}
			return isMatch;
		}

		public static MemberInfo[] GetMembersRecursive(this Type type, BindingFlags bindingAttr, Func<MemberInfo, bool> filter, Func<MemberInfo, bool> stopCondition, int maxDepth = 2)
		{
			List<MemberInfo> res = new List<MemberInfo>();
			foreach (MemberInfo member in type.GetMembers(bindingAttr).Where(filter))
			{
				res.Add(member);
				if (maxDepth == 0 || stopCondition(member))
					continue;
				if (member.MemberType == MemberTypes.Field)
					res.AddRange(((FieldInfo)member).FieldType.GetMembersRecursive(bindingAttr, filter, stopCondition, maxDepth - 1));
				else if (member.MemberType == MemberTypes.Property)
					res.AddRange(((PropertyInfo)member).PropertyType.GetMembersRecursive(bindingAttr, filter, stopCondition, maxDepth - 1));
			}
			return res.ToArray();
		}

	}

}
