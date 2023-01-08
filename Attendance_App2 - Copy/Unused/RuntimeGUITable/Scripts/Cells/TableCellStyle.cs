using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

namespace UnityUITable
{

	public abstract class TableCellStyle : ScriptableObject
	{
		List<FieldInfo> _autoSettingFields;
		public List<FieldInfo> AutoSettingFields
		{
			get
			{
				if (_autoSettingFields == null)
					_autoSettingFields = this.GetType().GetFields().Where(field => typeof(BaseSetting).IsAssignableFrom(field.FieldType)).ToList();
				return _autoSettingFields;
			}
		}
		List<BaseSetting> _autoSettings;
		public List<BaseSetting> AutoSettings
		{
			get
			{
				if (_autoSettings == null)
					_autoSettings = AutoSettingFields.Select(field => (BaseSetting)field.GetValue(this)).ToList();
				return _autoSettings;
			}
		}
	}

	public abstract class BaseSetting
	{
		public abstract void ApplySetting(TableCell cellContent);
		public abstract object GetDefaultValue(TableCell cellContent);
	}

	public class CellSpecificSetting<T, CellType> : BaseSetting where CellType : TableCell
	{
		public T value;
		public Func<CellType, T> defaultValueGetter;
		public Action<CellType, T> applySetting;
		public CellSpecificSetting(Action<CellType, T> setter, Func<CellType, T> getter)
		{
			this.applySetting = setter;
			this.defaultValueGetter = getter;
		}
		public override void ApplySetting(TableCell cellContent)
		{
			applySetting((CellType)cellContent, value);
		}
		public override object GetDefaultValue(TableCell cellContent)
		{
			return defaultValueGetter((CellType)cellContent);
		}
	}

	public class Setting<T> : CellSpecificSetting<T, TableCell>
	{
		public Setting(Action<TableCell, T> setter, Func<TableCell, T> getter) : base(setter, getter) { }
	}

	[Serializable]
	public class FontSetting : Setting<Font>
	{
		public FontSetting(Action<TableCell, Font> setter, Func<TableCell, Font> getter) : base(setter, getter) { }
	}
	[Serializable]
	public class FontStyleSetting : Setting<FontStyle>
	{
		public FontStyleSetting(Action<TableCell, FontStyle> setter, Func<TableCell, FontStyle> getter) : base(setter, getter) { }
	}
	[Serializable]
	public class ColorSetting : Setting<Color>
	{
		public ColorSetting(Action<TableCell, Color> setter, Func<TableCell, Color> getter) : base(setter, getter) { }
	}
	[Serializable]
	public class FloatSetting : Setting<float>
	{
		public FloatSetting(Action<TableCell, float> setter, Func<TableCell, float> getter) : base(setter, getter) { }
	}
	[Serializable]
	public class IntSetting : Setting<int>
	{
		public IntSetting(Action<TableCell, int> setter, Func<TableCell, int> getter) : base(setter, getter) { }
	}
	[Serializable]
	public class SpriteSetting : Setting<Sprite>
	{
		public SpriteSetting(Action<TableCell, Sprite> setter, Func<TableCell, Sprite> getter) : base(setter, getter) { }
	}
	[Serializable]
	public class BoolSetting : Setting<bool>
	{
		public BoolSetting(Action<TableCell, bool> setter, Func<TableCell, bool> getter) : base(setter, getter) { }
	}
	[Serializable]
	public class StringSetting : Setting<string>
	{
		public StringSetting(Action<TableCell, string> setter, Func<TableCell, string> getter) : base(setter, getter) { }
	}

}
