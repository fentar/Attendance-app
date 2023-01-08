using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

namespace UnityUITable
{

	[ExecuteInEditMode]
	public class CellContainer : MonoBehaviour
	{

		TableColumn _column;
		public TableColumn column
		{
			get
			{
				if (_column == null)
					_column = GetComponentInParent<TableColumn>();
				return _column;
			}
		}
		public Table table { get { return column.table; } }
		public virtual TableColumnInfo columnInfo { get { return column.info; } }
		public int rowIndex { get { return transform.GetSiblingIndex() + 1 - (table.hasTitles ? 1 : 0); } }

		[SerializeField][HideInInspector] LayoutElement layoutElement;
		public Transform content;
		[SerializeField][HideInInspector] TableCell _cellInstance;
		public TableCell cellInstance { get { return _cellInstance; } private set { _cellInstance = value; } }

		public float contentRequiredHeight
		{
			get
			{
				return cellInstance.contentRequiredHeight;
			}
		}

		public void Initialize()
		{
			layoutElement = gameObject.AddComponent<LayoutElement>();
			Update();
		}

		public void Initialize(int rowIndex)
		{
			SetRowIndex(rowIndex);
			gameObject.name += rowIndex;
			Initialize();
		}

		public TableCell CreateCellContent(TableCell cellPrefab)
		{
			cellInstance = GameObjectUtils.InstantiatePrefab(cellPrefab, content);
			return cellInstance;
		}

		void SetRowIndex(int rowIndex)
		{
			transform.SetSiblingIndex(rowIndex + 1);
		}

		protected virtual void Update()
		{
			if (table == null)
				return;
			if (table.horizontal)
				layoutElement.preferredWidth = table.GetHeight(rowIndex);
			else
				layoutElement.preferredHeight = table.GetHeight(rowIndex);
		}

		public virtual void UpdateContent()
		{
			if (cellInstance)
				cellInstance.UpdateContent();
		}

		public void UpdateStyle()
		{
			if (cellInstance)
			{
				if (!cellInstance.IsRightCellType)
				{
					TableCell rightCellPrefab = cellInstance.columnInfo.CellPrefab;
					int elmtIndex = cellInstance.elmtIndex;
					DestroyImmediate(cellInstance.gameObject);

					cellInstance = CreateCellContent(rightCellPrefab);
					cellInstance.Initialize();
				}
				cellInstance.UpdateStyle();
			}
		}

	}

}