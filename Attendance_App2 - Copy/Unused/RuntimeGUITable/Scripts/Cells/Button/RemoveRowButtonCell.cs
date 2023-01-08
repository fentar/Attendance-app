using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace UnityUITable
{
	
	public class RemoveRowButtonCell : ButtonCell 
	{

		protected override void OnButtonClicked ()
		{

			object obj = new PropertyOrFieldInfo(table.targetCollection.GetMember()).GetValue(table.targetCollection.GetComponent());
			MethodInfo deleteMethod = obj.GetType().GetMethod("RemoveAt");
			if (deleteMethod != null)
				deleteMethod.Invoke(obj, new object[] { elmtIndex });
			else
				Debug.LogError("There is no RemoveAt method on this collection.");
			
			table.UpdateContent();
			
		}

	}

}
