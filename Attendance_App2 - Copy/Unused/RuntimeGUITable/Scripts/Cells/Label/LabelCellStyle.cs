using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

namespace UnityUITable
{

	public class LabelCellStyle : TableCellStyle
	{

		[Serializable]
		public class FontDataSetting : CellSpecificSetting<FontData, LabelCell>
		{
			public FontDataSetting(Action<LabelCell, FontData> applySetting, Func<LabelCell, FontData> defaultValueGetter) : base(applySetting, defaultValueGetter) { }
		}

		public FontDataSetting fontData = new FontDataSetting(
			(cell, v) =>
			{
				Text text = cell.label;
				text.font = v.font;
				text.fontSize = v.fontSize;
				text.fontStyle = v.fontStyle;
				text.resizeTextForBestFit = v.bestFit;
				text.resizeTextMinSize = v.minSize;
				text.resizeTextMaxSize = v.maxSize;
				text.alignment = v.alignment;
				text.alignByGeometry = v.alignByGeometry;
				text.supportRichText = v.richText;
				text.horizontalOverflow = v.horizontalOverflow;
				text.verticalOverflow = v.verticalOverflow;
				text.lineSpacing = v.lineSpacing;
			},
			(cell =>
			{
				Text text = cell.label;
				FontData res = FontData.defaultFontData;
				res.font = text.font;
				res.fontSize = text.fontSize;
				res.fontStyle = text.fontStyle;
				res.bestFit = text.resizeTextForBestFit;
				res.minSize = text.resizeTextMinSize;
				res.maxSize = text.resizeTextMaxSize;
				res.alignment = text.alignment;
				res.alignByGeometry = text.alignByGeometry;
				res.richText = text.supportRichText;
				res.horizontalOverflow = text.horizontalOverflow;
				res.verticalOverflow = text.verticalOverflow;
				res.lineSpacing = text.lineSpacing;
				return res;
			}));

		public ColorSetting color = new ColorSetting(
			(cell, v) => ((LabelCell)cell).label.color = v,
			cell => ((LabelCell)cell).label.color);

	}

}
