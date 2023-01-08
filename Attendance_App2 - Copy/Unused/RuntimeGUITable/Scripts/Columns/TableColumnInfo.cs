using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;

namespace UnityUITable
{

	[System.Serializable]
	public class TableColumnInfo
	{

		// Unity's serialization doesn't respect inheritance, so we can't use it to differentiate Add & Delete Info
		// When code is recompiled, the subclasses instances will become instances of this mother class and lose all overriden behaviour.
		// So we use this dirty enum and switch/cases on required properties to simulate sub classes.
		// We kept the actual subclasses so from the outside, it will look like proper OOP. The subclasses will set this enum value.
		// We kept the differing properties virtual for reference and hilight.
		public enum CellInfoType { Normal, Add, Delete }
		public CellInfoType infoType;

		const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

		public string fieldName;
		public float width = 100f;
		public bool autoWidth;
		public bool useRelativeWidth;
		[SerializeField] private TableCell cellPrefab;
		public virtual TableCell CellPrefab 
		{ 
			get 
			{  
				switch(infoType)
				{
					case CellInfoType.Add:
						return table.addCellPrefab;
					case CellInfoType.Delete:
						return table.deleteCellPrefab;
					default:
						return cellPrefab;
				}

			} 
		}
		public bool isSortable;
		public bool expandableHeight;
		public bool autoColumnTitle = true;
		public string columnTitle;
		public bool useColumnTitleImage;
		public Sprite columnTitleImage;
		[SerializeField] private TableCellStyle cellStyle;
		public virtual TableCellStyle CellStyle
		{
			get
			{
				switch (infoType)
				{
					case CellInfoType.Add:
						return table.AddCellStyle;
					case CellInfoType.Delete:
						return table.DeleteCellStyle;
					default:
						return cellStyle;
				}
			}
		}
		 
		public Table table;

		float absoluteWidth;
		public virtual float AbsoluteWidth 
		{
			get
			{
				switch (infoType)
				{
					case CellInfoType.Add:
						return 0f;
					case CellInfoType.Delete:
						return table.deleteColumnWidth;
					default:
						return absoluteWidth;
				}
			}
			set
			{
				absoluteWidth = value;
			}
		}

		public System.Type elementType { get { return table == null ? null : table.ElementType; } }

		public MemberInfo GetMember()
		{
			if (elementType == null) return null;
			if (string.IsNullOrEmpty(fieldName)) return null;
			MemberInfo[] members = elementType.GetMember(fieldName, bindingFlags);
			if (members == null || members.Length == 0) return null;
			return members.FirstOrDefault(IsValidMember);
		}

		public virtual bool IsDefined
		{
			get
			{
				switch (infoType)
				{
					case CellInfoType.Add:
					case CellInfoType.Delete:
						return true;
					default:
						return IsMemberValid && cellPrefab != null;
				}
			}
		}

		public bool IsMemberValid
		{
			get
			{
				return IsValidMember(GetMember());
			}
		}

		public bool IsValidMember(MemberInfo member)
		{
			if (member == null)
				return false;
			if (member.MemberType != MemberTypes.Field && member.MemberType != MemberTypes.Property && member.MemberType != MemberTypes.Method)
				return false;
			if (member.GetCustomAttributes(false).OfType<System.ObsoleteAttribute>().Count() > 0)
				return false;
			if (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)
				return true;
			if (member.MemberType == MemberTypes.Method)
			{
				MethodInfo method = (MethodInfo)member;
				return !method.IsSpecialName && method.DeclaringType == elementType && method.GetParameters().Length == 0;
			}
			return false;
		}

		int Order(MemberInfo member)
		{
			if (member.MemberType == MemberTypes.Field)
				return 0;
			if (member.MemberType == MemberTypes.Property)
				return 1;
			return 2;
		}

		public MemberInfo[] GetValidMembers()
		{
			return elementType
				.GetMembers(bindingFlags)
				.Where(IsValidMember)
				.OrderBy(Order)
				.ToArray();
		}

		public TableColumnInfo()
		{
			this.infoType = CellInfoType.Normal;
		}

	}

}
