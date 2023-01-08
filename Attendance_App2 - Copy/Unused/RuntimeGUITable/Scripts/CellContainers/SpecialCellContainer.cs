using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUITable
{

	public class SpecialCellContainer : CellContainer
	{

		[SerializeField]
		int columnInfoIndex;

		public override TableColumnInfo columnInfo { get { return table.GetColumnInfoAt(columnInfoIndex); } }

	}

}
