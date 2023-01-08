using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;

using System;

namespace UnityUITable
{

	[CustomPropertyDrawer(typeof(UnityMemberInfo))]
	public class UnityMemberInfoDrawer : PropertyDrawer
	{

		const string TARGET = "target";
		const string COMPONENT = "componentName";
		const string MEMBER = "memberName";

		bool changed;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			UnityMemberInfo unityMemberInfo = (UnityMemberInfo)property.GetTargetObjectOfProperty();

			SerializedProperty targetProperty = property.FindPropertyRelative(TARGET);
			SerializedProperty componentProperty = property.FindPropertyRelative(COMPONENT);
			SerializedProperty memberProperty = property.FindPropertyRelative(MEMBER);

			Rect objPos = new Rect(position.x, position.y, position.width * 0.4f, position.height);
			Rect propPos = new Rect(position.x + position.width * 0.4f + 4f, position.y, position.width * 0.6f - 4f, position.height);
			Rect boxPos = EditorGUI.IndentedRect(objPos);
			GUI.Box(boxPos, GUIContent.none);
			EditorGUI.PropertyField(objPos, targetProperty, new GUIContent(""));

			GameObject targetGO = (GameObject)targetProperty.objectReferenceValue;
			if (targetGO != null)
			{
				Component[] components = targetGO.GetComponents<Component>();

				GUIContent buttonContent;

				if (string.IsNullOrEmpty(memberProperty.stringValue))
					buttonContent = new GUIContent("No Collection");
				else if (!((UnityMemberInfo)property.GetTargetObjectOfProperty()).IsDefined)
					buttonContent = new GUIContent(string.Format("<Missing: {0}.{1}>", componentProperty.stringValue, memberProperty.stringValue));
				else
					buttonContent = new GUIContent(string.Format("{0}.{1}", componentProperty.stringValue, memberProperty.stringValue));
				bool changed = GUI.changed;
				if (GUI.Button(propPos, buttonContent, EditorStyles.popup))
				{
					var menu = new GenericMenu();

					menu.AddItem(
						new GUIContent("No Collection"),
						string.IsNullOrEmpty(memberProperty.stringValue),
						() => 
						{
							if (!string.IsNullOrEmpty(componentProperty.stringValue))
							{
								componentProperty.stringValue = memberProperty.stringValue = string.Empty;
								property.serializedObject.ApplyModifiedProperties();
								this.changed = true;
							}
						});
					menu.AddSeparator("");
					foreach (Component comp in components)
					{
						string compName = comp.GetType().Name;

						MemberInfo[] propertyOptions = comp.GetType().GetMembers()
							.Where(mi => unityMemberInfo.MemberFilter(mi))
							.ToArray();

						foreach(MemberInfo prop in propertyOptions)
						{
							string propName = prop.Name;
							menu.AddItem(
								new GUIContent(compName + "/" + propName),
								componentProperty.stringValue == compName && memberProperty.stringValue == propName,
								() => 
								{
									if (componentProperty.stringValue != compName || memberProperty.stringValue != propName)
									{
										componentProperty.stringValue = compName;
										memberProperty.stringValue = propName;
										property.serializedObject.ApplyModifiedProperties();
										this.changed = true;
									}
								});
						}
					}
					menu.DropDown(propPos);
				}
				GUI.changed = this.changed || changed;
				if (this.changed) 
					this.changed = false;

			}
			else
			{
				GUI.enabled = false;
				GUI.Button(propPos, new GUIContent("No Collection"), EditorStyles.popup);
				GUI.enabled = true;
			}
			property.serializedObject.ApplyModifiedProperties();

			EditorGUI.indentLevel--;

			EditorGUI.EndProperty();

		}

	}

}
