using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityUITable
{

	public class ButtonCellStyle : TableCellStyle
	{

		public StringSetting text = new StringSetting(
			(cell, v) => cell.GetComponentInChildren<Text>().text = v,
			(cell) => cell.GetComponentInChildren<Text>().text);

		public ColorSetting textColor = new ColorSetting(
			(cell, v) => cell.GetComponentInChildren<Text>().color = v,
			(cell) => cell.GetComponentInChildren<Text>().color);

		public FontSetting textFont = new FontSetting(
			(cell, v) => cell.GetComponentInChildren<Text>().font = v,
			(cell) => cell.GetComponentInChildren<Text>().font);

		public ColorSetting bgColor = new ColorSetting(
			(cell, v) => cell.GetComponent<Image>().color = v,
			(cell) => cell.GetComponent<Image>().color);

	}

}
