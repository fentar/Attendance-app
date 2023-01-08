using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityUITable
{

	public class ButtonCell : StyleableTableCell<ButtonCellStyle>
	{

		public Button button;

		bool subscribed = false;

		public override int GetPriority(MemberInfo member)
		{
			return 10;
		}

		public override bool IsCompatibleWithMember(MemberInfo member)
		{
			return (member.MemberType == MemberTypes.Method && ((MethodInfo)member).GetParameters().Length == 0);
		}

		void Update()
		{
			if (!subscribed)
			{
				button.onClick.AddListener(OnButtonClicked);
				subscribed = true;
			}
		}

		public override void UpdateContent () { }

		protected virtual void OnButtonClicked()
		{
			((MethodInfo)member).Invoke(obj, new object[] { });
		}

	}

}
