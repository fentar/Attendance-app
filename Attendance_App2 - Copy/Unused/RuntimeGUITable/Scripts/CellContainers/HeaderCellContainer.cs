using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using UnityEngine.EventSystems;

namespace UnityUITable
{

	[ExecuteInEditMode]
	public class HeaderCellContainer : CellContainer
	{

		public Text label;
		public Image icon;
		public Image background;
		public Image outline;
		public Image sortModeIcon;

		Button _button;
		Button button
		{
			get
			{
				if (_button == null) _button = GetComponent<Button>();
				return _button;
			}
		}

		protected override void Update()
		{
			if (table == null)
				return;
			base.Update();
			label.color = table.titleFontColor;
			label.font = table.titleFont;
			label.fontStyle = table.titleFontStyle;
			label.fontSize = table.titleFontSize;
			sortModeIcon.color = table.titleFontColor;
			background.color = table.titleBGColor;
			outline.color = table.lineColor;
			outline.sprite = table.outlineSprite;
			button.enabled = columnInfo.isSortable;
			if (columnInfo.useColumnTitleImage)
			{
				label.gameObject.SetActive(false);
				icon.gameObject.SetActive(true);
				icon.sprite = columnInfo.columnTitleImage;
			}
			else
			{
				icon.gameObject.SetActive(false);
				label.gameObject.SetActive(true);
				label.text = columnInfo.columnTitle;
			}
			UpdateSortModeIcon();
		}

		void UpdateSortModeIcon()
		{
			if (table.sortingState.sortingColumn != column.info)
			{
				sortModeIcon.enabled = false;
			}
			else if (table.sortingState.sortMode == SortingState.SortMode.Ascending)
			{
				sortModeIcon.enabled = true;
				sortModeIcon.rectTransform.localScale = Vector3.one;
			}
			else if (table.sortingState.sortMode == SortingState.SortMode.Descending)
			{
				sortModeIcon.enabled = true;
				sortModeIcon.rectTransform.localScale = new Vector3(1f, -1f, 1f);
			}
			else
			{
				sortModeIcon.enabled = false;
			}
		}

		public void OnTitleClicked()
		{
			column.TitleClicked();
		}

	}

}