using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

namespace UnityUITable
{

	[ExecuteInEditMode]
	public class DeleteColumn : TableColumn
	{

		protected override CellContainer TryFindTitleCell()
		{
			if (transform.childCount > 0)
			{
				CellContainer firstCell = transform.GetChild(0).GetComponent<CellContainer>();
				if (firstCell != cellContainers[0])
					return firstCell;
			}
			return null;
		}

		protected override CellContainer CreateCell(int rowIndex)
		{
			CellContainer cellContainer = GameObjectUtils.InstantiatePrefab(table.emptyCellContainerPrefab, transform);
			cellContainer.Initialize(rowIndex);
			cellContainers.Add(cellContainer);
			ButtonCell cell = (ButtonCell)cellContainer.CreateCellContent(info.CellPrefab);
			cell.Initialize();
			return cellContainer;
		}

		protected override void CreateTitleCell()
		{
			titleButtonCellContainer = AddEmptyCell("Empty Title");
			titleButtonCellContainer.transform.SetSiblingIndex(0);
		}

	}

}
