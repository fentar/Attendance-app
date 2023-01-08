using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using System;

namespace UnityUITable
{

	public abstract class StyleableTableCell<StyleTypeT> : TableCell where StyleTypeT : TableCellStyle
	{

		public sealed override Type StyleType
		{
			get
			{
				return typeof(StyleTypeT);
			}
		}

		public sealed override void UpdateStyle()
		{
			if (cellStyle == null)
				return;
			if (!typeof(StyleTypeT).IsInstanceOfType(cellStyle))
				return;
			StyleTypeT style = (StyleTypeT)cellStyle;
			foreach (BaseSetting setting in style.AutoSettings)
			{
				setting.ApplySetting(this);
			}
			ApplyCustomStyle(style);
		}

		protected virtual void ApplyCustomStyle(StyleTypeT style) { }

		public void SetDefaultSettings(StyleTypeT style)
		{
			foreach (FieldInfo fieldInfo in style.AutoSettingFields)
			{
				BaseSetting setting = ((BaseSetting)fieldInfo.GetValue(style));
				object defaultValue = setting.GetDefaultValue(this);
				fieldInfo.FieldType.GetField("value").SetValue(fieldInfo.GetValue(style), defaultValue);
			}
			SetDefaultCustomSettings(style);
		}
		protected virtual void SetDefaultCustomSettings(StyleTypeT style) { }

	}

}
