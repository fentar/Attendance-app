using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace UnityUITable
{

	public class ImageCell : StyleableTableCell<ImageCellStyle>
	{

		public override bool IsCompatibleWithMember(MemberInfo member)
		{
			return IsOfCompatibleType(member, typeof(Texture2D), typeof(Sprite));
		}

		public override int GetPriority(MemberInfo member)
		{
			return 10;
		}

		public Image image;

		public override void UpdateContent()
		{
			if (property == null || property.IsEmpty)
				return;
			object v = property.GetValue(obj);
			if (v is Sprite)
				image.sprite = (Sprite)v;
			else if (v is Texture2D)
			{
				Texture2D tex2D = (Texture2D)v;
				image.sprite = Sprite.Create(tex2D, new Rect(0f, 0f, tex2D.width, tex2D.height), 0.5f * Vector2.one);
			}
		}

	}

}
