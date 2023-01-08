using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityEngine.UI;

namespace UnityUITable
{

	[ExecuteInEditMode]
	public abstract class TableCell : MonoBehaviour
	{
		class RectTransformParam
		{
			public Vector2 anchorMin;
			public Vector2 anchorMax;
			public Vector2 anchoredPosition;
			public Vector2 pivot;
			public RectTransformParam(RectTransform rectTransform)
			{
				this.anchorMin = rectTransform.anchorMin;
				this.anchorMax = rectTransform.anchorMax;
				this.anchoredPosition = rectTransform.anchoredPosition;
				this.pivot = rectTransform.pivot;
			}
			public void Apply(RectTransform rectTransform)
			{
				rectTransform.anchorMin = this.anchorMin;
				rectTransform.anchorMax = this.anchorMax;
				rectTransform.anchoredPosition = this.anchoredPosition;
				rectTransform.pivot = this.pivot;
			}
		}

		public RectTransform expandableElement;
		ContentSizeFitter expandFitter;
		RectTransformParam savedRectTransform;
		bool isSetExpandable { get { return expandFitter != null && expandableFit == ContentSizeFitter.FitMode.PreferredSize; } }

			CellContainer _container;
		public CellContainer container
		{
			get
			{
				if (_container == null)
					_container = transform.parent.GetComponent<CellContainer>();
				if (_container == null)
					_container = transform.parent.parent.parent.GetComponent<CellContainer>();
				return _container;
			}
		}

		public Table table { get { return container.table; } }
		public TableColumn column { get { return container.column; } }

		MemberInfo _member;
		protected MemberInfo member
		{
			get
			{
				if (_member == null)
					_member = columnInfo.GetMember();
				return _member;
			}
		}
		public int elmtIndex { get { return container.rowIndex - 1; } }
		public object obj { get { return table.GetSortedElements()[elmtIndex]; } }

		public TableColumnInfo columnInfo { get { return container.columnInfo; } }
		protected TableCellStyle cellStyle { get { return columnInfo.CellStyle; } }

		PropertyOrFieldInfo _property;
		protected PropertyOrFieldInfo property
		{
			get
			{
				if (_property == null) _property = new PropertyOrFieldInfo(member);
				return _property;
			}
		}

		public float contentRequiredHeight
		{
			get
			{
				if (isSetExpandable)
					return (table.horizontal ? expandableElement.rect.width : expandableElement.rect.height) + 17f;
				else
					return -1f;
			}
		}

		ContentSizeFitter.FitMode expandableFit
		{
			get { return table.horizontal ? expandFitter.horizontalFit : expandFitter.verticalFit; }
			set 
			{ 
				if (table.horizontal)
					expandFitter.horizontalFit = value;
				else
					expandFitter.verticalFit = value;
			}
		}

		public virtual Type StyleType { get { return null; } }

		protected static bool IsOfCompatibleType(MemberInfo member, params Type[] compatibleTypes)
		{
			return compatibleTypes != null && compatibleTypes.Any(t => t.IsAssignableFrom(PropertyOrFieldInfo.GetPropertyOrFieldType(member)));
		}

		public abstract bool IsCompatibleWithMember(MemberInfo member);

		/// <summary>
		/// Override to define the priority of the cell type, depending on the member associated.
		/// </summary>
		/// <returns>The priority</returns>
		/// <param name="member">The member associated with this column.</param>
		public virtual int GetPriority(MemberInfo member)
		{
			return 0;
		}

		public void Initialize()
		{
			UpdateContent();
			UpdateStyle();
		}

		public abstract void UpdateContent();

		public virtual void UpdateStyle() { }

		public bool IsRightCellType
		{
			get
			{
				return name.StartsWith(columnInfo.CellPrefab.name);
			}
		}

		private void Update()
		{
			SetExpandable(columnInfo.expandableHeight);
		}

		public void SetExpandable(bool enable)
		{
			if (expandableElement != null && expandFitter == null)
			{
				expandFitter = expandableElement.GetOrAddComponent<ContentSizeFitter>();
			}
			if (expandFitter != null)
			{
				if (enable && expandableFit != ContentSizeFitter.FitMode.PreferredSize)
				{
					savedRectTransform = new RectTransformParam(expandableElement);
					expandableFit = ContentSizeFitter.FitMode.PreferredSize;
				}
				else if (!enable && expandableFit != ContentSizeFitter.FitMode.Unconstrained)
				{
					expandableFit = ContentSizeFitter.FitMode.Unconstrained;
					if (savedRectTransform != null)
						savedRectTransform.Apply(expandableElement);
				}
			}
		}

	}

}
