using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityUITable
{

	public class ToggleCellStyle : InteractableCellStyle
	{

		public ColorSetting bgColor = new ColorSetting(
			(cell, v) => cell.transform.Find("Background").GetComponent<Image>().color = v,
			(cell) => cell.transform.Find("Background").GetComponent<Image>().color);

		public ColorSetting checkColor = new ColorSetting(
			(cell, v) => ((ToggleCell)cell).toggle.graphic.color = v,
			(cell) => ((ToggleCell)cell).toggle.graphic.color);

		public SpriteSetting bgSprite = new SpriteSetting(
			(cell, v) => cell.transform.Find("Background").GetComponent<Image>().sprite = v,
			(cell) => cell.transform.Find("Background").GetComponent<Image>().sprite);

		public SpriteSetting checkSprite = new SpriteSetting(
			(cell, v) => ((Image)((ToggleCell)cell).toggle.graphic).sprite = v,
			(cell) => ((Image)((ToggleCell)cell).toggle.graphic).sprite);



	}

}
