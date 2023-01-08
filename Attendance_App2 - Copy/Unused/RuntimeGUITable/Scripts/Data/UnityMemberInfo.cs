using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

namespace UnityUITable
{

	[System.Serializable]
	public class UnityMemberInfo
	{

		public GameObject target;
		public string componentName;
		public string memberName;

		System.Func<MemberInfo, bool> memberFilter;
		public System.Func<MemberInfo, bool> MemberFilter { get { return memberFilter; } }

		public UnityMemberInfo(System.Func<MemberInfo, bool> memberFilter)
		{
			this.memberFilter = memberFilter;
		}

		ICollection _cachedCollection;
		System.Type _cachedElementType;
		MemberInfo _cachedMember;
		Component _cachedComponent;


		GameObject _cachedCollectionTarget;
		string _cachedCollectionComponentName;
		string _cachedCollectionMemberName;

		public bool IsDefined
		{
			get
			{
				return target != null && GetComponent() != null && GetMember() != null;
			}
		}

		bool IsCacheOutOfDate
		{
			get
			{
				return (_cachedCollection == null || _cachedCollectionTarget != target || _cachedCollectionComponentName != componentName || _cachedCollectionMemberName != memberName);
			}
		}

		public void UpdateCache()
		{
			try { _cachedComponent = target.GetComponent(componentName); } catch { _cachedComponent = null; }
			try { _cachedMember = _cachedComponent.GetType().GetMember(memberName)[0]; } catch { _cachedMember = null; }
			PropertyOrFieldInfo propertyOrField = new PropertyOrFieldInfo(_cachedMember);
			object obj = propertyOrField.GetValue(_cachedComponent);
			_cachedCollection = (ICollection)obj;
			_cachedElementType = _cachedMember == null ? null : propertyOrField.Type.GetGenericArguments()[0];
			_cachedCollectionTarget = target;
			_cachedCollectionComponentName = componentName;
			_cachedCollectionMemberName = memberName;
		}

		public MemberInfo GetMember()
		{
			if (IsCacheOutOfDate)
			{
				UpdateCache();
			}
			return _cachedMember;
		}

		public Component GetComponent()
		{
			if (IsCacheOutOfDate)
			{
				UpdateCache();
			}
			return _cachedComponent;
		}

		public ICollection Collection
		{
			get
			{
				if (IsCacheOutOfDate)
				{
					UpdateCache();
				}
				return _cachedCollection;
			}
		}
		public System.Type ElementType
		{
			get
			{
				if (IsCacheOutOfDate)
				{
					UpdateCache();
				}
				return _cachedElementType;
			}
		}

	}

}