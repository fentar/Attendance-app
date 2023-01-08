using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUITable
{

	public class SortingState
	{
		public enum SortMode { None, Ascending, Descending }
		public SortMode sortMode { get; set; }
		public TableColumnInfo sortingColumn { get; set; }
		public SortingState()
		{
			this.sortMode = SortMode.None;
		}
		public SortingState(TableColumnInfo sortingColumn, SortMode sortMode)
		{
			this.sortingColumn = sortingColumn;
			this.sortMode = sortMode;
		}
		public void ClickOnColumn(TableColumnInfo column)
		{
			if (column == sortingColumn)
				sortMode = (SortMode)((((int)sortMode) + 1) % System.Enum.GetValues(typeof(SortMode)).Length);
			else
			{
				sortingColumn = column;
				sortMode = SortMode.Ascending;
			}
		}
		public object KeySelector(object elmt)
		{
			PropertyOrFieldInfo property = new PropertyOrFieldInfo(elmt.GetType().GetMember(sortingColumn.fieldName)[0]);
			return property.GetValue(elmt);
		}
	}

}
