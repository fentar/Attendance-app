using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace UnityUITable
{

	public class InputCellStyle : InteractableCellStyle
	{

		[Serializable]
		public class LineTypeSetting : CellSpecificSetting<InputField.LineType, InputCell>
		{
			public LineTypeSetting(Action<InputCell, InputField.LineType> applySetting, Func<InputCell, InputField.LineType> defaultValueGetter) : base(applySetting, defaultValueGetter) { }
		}

		public FontSetting font = new FontSetting(
			(cell, v) =>
			{
				InputField inputField = ((InputCell)cell).inputField;
				inputField.textComponent.font = v;
				if (inputField.placeholder != null && inputField.placeholder is Text) ((Text)inputField.placeholder).font = v;
			},
			(cell => ((InputCell)cell).inputField.textComponent.font));

		public ColorSetting bgColor = new ColorSetting(
			(cell, v) => ((InputCell)cell).inputField.GetComponent<Image>().color = v,
			(cell => ((InputCell)cell).inputField.GetComponent<Image>().color));

		public ColorSetting textColor = new ColorSetting(
			(cell, v) =>
			{
				InputField inputField = ((InputCell)cell).inputField;
				inputField.textComponent.color = v;
				if (inputField.placeholder != null) inputField.placeholder.color = v;
			},
			(cell => ((InputCell)cell).inputField.textComponent.color));

		public LineTypeSetting lineType = new LineTypeSetting(
			(cell, v) => ((InputCell)cell).inputField.lineType = v,
			(cell => ((InputCell)cell).inputField.lineType));


	}

}
