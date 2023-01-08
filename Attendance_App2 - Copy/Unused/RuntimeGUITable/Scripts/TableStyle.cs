using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUITable
{

	[CreateAssetMenu]
	public class TableStyle : ScriptableObject
	{

		public enum RowsColorMode { Plain, Striped }
		public RowsColorMode rowsColorMode;
		public Color bgColor = Color.gray;
		public Color altBgColor = Table.DARK_GRAY;

		public bool selectableRows;
		public Color selectedBgColor = Table.SELECTION_BLUE;

		public Color lineColor = Color.white;
		public Color titleBGColor = Table.DARK_BLUE;
		public Color titleFontColor = Color.white;
		public int titleFontSize = 14;

		public float rowHeight = 32f;
		public float spacing = -1f;

		public Sprite outlineSprite;

	}

}
