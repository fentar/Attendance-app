using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUITable
{

	public class AddCellInfo : TableColumnInfo
	{
		 
		public AddCellInfo(Table table)
		{
			this.infoType = CellInfoType.Add;
			this.table = table;
		}

	}

}
