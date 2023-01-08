using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityUITable
{

	public class DropdownCellStyle : InteractableCellStyle
	{


		public FontSetting font = new FontSetting(
			(cell, v) => ((DropdownCell)cell).dropdown.captionText.font = ((DropdownCell)cell).dropdown.itemText.font = v,
			cell => ((DropdownCell)cell).dropdown.captionText.font);

		public IntSetting fontSize = new IntSetting(
			(cell, v) => ((DropdownCell)cell).dropdown.captionText.fontSize = ((DropdownCell)cell).dropdown.itemText.fontSize = v,
			cell => ((DropdownCell)cell).dropdown.captionText.fontSize);

		public ColorSetting buttonBGColor = new ColorSetting(
			(cell, v) => cell.GetComponent<Image>().color = v,
			cell => cell.GetComponent<Image>().color);

		public ColorSetting buttonFontColor = new ColorSetting(
			(cell, v) => ((DropdownCell)cell).dropdown.captionText.color = v,
			cell => ((DropdownCell)cell).dropdown.captionText.color);

		public ColorSetting dropdownBGColor = new ColorSetting(
			(cell, v) =>
			{
				((DropdownCell)cell).dropdown.template.GetComponent<Image>().color = v;
				((DropdownCell)cell).dropdown.template.GetComponentInChildren<Toggle>().targetGraphic.color = v;
			},
			cell => ((DropdownCell)cell).dropdown.template.GetComponent<Image>().color);


		public ColorSetting dropdownFontColor = new ColorSetting(
			(cell, v) => ((DropdownCell)cell).dropdown.itemText.color = ((DropdownCell)cell).dropdown.template.GetComponentInChildren<Toggle>().graphic.color = v,
			cell => ((DropdownCell)cell).dropdown.itemText.color);

	}

}
