using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityUITable
{

	[CustomPropertyDrawer(typeof(BaseSetting), true)]
	public class SettingDrawer : PropertyDrawer
	{

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.PropertyField(position, property.FindPropertyRelative("value"), new GUIContent(property.displayName));
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{

			return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("value"));
		}

	}

}
