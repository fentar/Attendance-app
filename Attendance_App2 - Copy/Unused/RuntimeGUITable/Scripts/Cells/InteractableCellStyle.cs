using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityUITable
{

	public abstract class InteractableCellStyle : TableCellStyle
	{

		public BoolSetting interactable = new BoolSetting(
			(cell, v) =>
			{
				cell.GetComponentsInChildren<Graphic>().ForEach(g => g.raycastTarget = v);
				((InteractableCellInterface)cell).interactable = v;
			},
			(cell) => cell.GetComponentInChildren<Graphic>().raycastTarget);

	}

}
