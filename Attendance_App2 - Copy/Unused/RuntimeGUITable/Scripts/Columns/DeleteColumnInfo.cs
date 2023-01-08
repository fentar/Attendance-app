using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUITable
{

	public class DeleteColumnInfo : TableColumnInfo
	{

		public DeleteColumnInfo(Table table)
		{
			this.infoType = CellInfoType.Delete;
			this.table = table;
			this.autoColumnTitle = false;
			this.columnTitle = "";
		}

	}

}
