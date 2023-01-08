using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace UnityUITable
{

	public class ImageCellStyle : TableCellStyle
	{

		[Serializable]
		public class MaterialSetting : CellSpecificSetting<Material, ImageCell>
		{
			public MaterialSetting(Action<ImageCell, Material> applySetting, Func<ImageCell, Material> defaultValueGetter) : base(applySetting, defaultValueGetter) { }
		}

		[Serializable]
		public class ImageTypeSetting : CellSpecificSetting<Image.Type, ImageCell>
		{
			public ImageTypeSetting(Action<ImageCell, Image.Type> applySetting, Func<ImageCell, Image.Type> defaultValueGetter) : base(applySetting, defaultValueGetter) { }
		}

		public FloatSetting padding = new FloatSetting(
			(cell, v) =>
			{
				cell.GetComponent<RectTransform>().offsetMin = Vector2.one * v;
				cell.GetComponent<RectTransform>().offsetMax = -Vector2.one * v;
			},
			cell => cell.GetComponent<RectTransform>().offsetMin.x);

		public ColorSetting color = new ColorSetting(
			(cell, v) => ((ImageCell)cell).image.color = v,
			cell => ((ImageCell)cell).image.color);

		public MaterialSetting material = new MaterialSetting(
			(cell, v) => cell.image.material = v,
			cell => cell.image.material);

		public ImageTypeSetting imageType = new ImageTypeSetting(
			(cell, v) => cell.image.type = v,
			cell => cell.image.type);

		public BoolSetting preserveAspect = new BoolSetting(
			(cell, v) => ((ImageCell)cell).image.preserveAspect = v,
			cell => ((ImageCell)cell).image.preserveAspect);



	}

}
