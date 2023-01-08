using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace UnityUITable
{

	public class AddRowButtonCell : ButtonCell 
	{

		protected override void OnButtonClicked ()
		{
			object obj = new PropertyOrFieldInfo(table.targetCollection.GetMember()).GetValue(table.targetCollection.GetComponent());
			MethodInfo addMethod = obj.GetType().GetMethod("Add");
			ConstructorInfo constructor = table.ElementType.GetConstructor(new System.Type[] { });
			if (constructor == null)
			{
				Debug.LogError("There is no default constructor on the collection's element type.");
				return;
			}
			object newElmt = constructor.Invoke(new object[] { });
			if (addMethod == null)
			{
				Debug.LogError("There is no Add method on this collection.");
				return;
			}
			addMethod.Invoke(obj, new object[] { newElmt });

			table.UpdateContent();

		}

	}

}