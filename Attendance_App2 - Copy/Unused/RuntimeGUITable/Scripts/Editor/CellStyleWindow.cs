using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace UnityUITable
{

	public class CellStyleWindow : EditorWindow
	{
		const string GENERATED_FILES_FOLDER = "Assets/RuntimeGUITable/Cell Styles/";

		SerializedProperty styleProperty;
		Table table;
		MemberInfo memberInfo;
		TableCell cellPrefab;
		ScriptableObject template;
		string filename;

		string stylePropertyPath;

		public static CellStyleWindow CreateStyleWindow(SerializedProperty styleProperty, Table table, MemberInfo memberInfo, TableCell cellPrefab, ScriptableObject template, string filename)
		{
			CellStyleWindow window = new CellStyleWindow();
			window.titleContent = new GUIContent((memberInfo == null) ? "Style" : memberInfo.Name + " Style");
			window.styleProperty = styleProperty;
			window.stylePropertyPath = styleProperty.propertyPath;
			window.table = table;
			window.memberInfo = memberInfo;
			window.cellPrefab = cellPrefab;
			window.template = template;
			window.filename = filename;
			window.ShowUtility();
			return window;
		}

		private void OnGUI()
		{
			if (table == null)
			{
				Close();
				return;
			}
			styleProperty = new SerializedObject(table).FindProperty(stylePropertyPath);
			try
			{
				if (styleProperty.objectReferenceValue == null)
					NoStyleGUI();
				else
					StyleGUI();
			}
			catch (ExitGUIException e)
			{
				throw e;
			}
			catch (System.Exception e)
			{
				Debug.Log("Received: " + e.Message + "\nClosing the style window.");
				Close();
			}
		}

		void CreateNew()
		{
			ScriptableObject tmpStyleObject;
			if (template == null)
			{
				tmpStyleObject = CreateInstance(cellPrefab.StyleType);
				cellPrefab.GetType().GetMethod("SetDefaultSettings").Invoke(cellPrefab, new object[] { tmpStyleObject });
			}
			else
				tmpStyleObject = Instantiate(template);
			System.IO.Directory.CreateDirectory(GENERATED_FILES_FOLDER);
			string path = EditorUtility.SaveFilePanel("Save Style...", GENERATED_FILES_FOLDER, filename + ".asset", "asset");
			if (string.IsNullOrEmpty(path))
				return;
			else if (path.StartsWith(Application.dataPath))
				path = "Assets" + path.Substring(Application.dataPath.Length);
			else
			{
				Debug.LogError("Cannot save asset outside of project folder.");
				return;
			}
			AssetDatabase.CreateAsset(tmpStyleObject, path);
			tmpStyleObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
			styleProperty.objectReferenceValue = tmpStyleObject;
			styleProperty.serializedObject.ApplyModifiedProperties();
		}

		void NoStyleGUI()
		{
			styleProperty.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("Import Style File"), styleProperty.objectReferenceValue, cellPrefab.StyleType, false);
			styleProperty.serializedObject.ApplyModifiedProperties();
			GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
			centeredStyle.alignment = TextAnchor.MiddleCenter;
			EditorGUILayout.LabelField(new GUIContent("or"), centeredStyle);
			if (GUILayout.Button("Create New Style File"))
			{
				CreateNew();
			}
			styleProperty.serializedObject.ApplyModifiedProperties();
		}

		void StyleGUI()
		{
			Object oldStyle = styleProperty.objectReferenceValue;
			styleProperty.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("Style File"), styleProperty.objectReferenceValue, cellPrefab.StyleType, false);
			styleProperty.serializedObject.ApplyModifiedProperties();
			if (styleProperty.objectReferenceValue != oldStyle && table != null)
				table.SetDirty();
			if (styleProperty.objectReferenceValue == null)
				return;
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			SerializedObject serializedObject = new SerializedObject(styleProperty.objectReferenceValue);
			SerializedProperty iterator = serializedObject.GetIterator();
			while (iterator.NextVisible(true))
			{
				if (iterator.name == "m_Script" || iterator.propertyPath.Contains("."))
					continue;
				if (iterator.name == "interactable" && memberInfo != null && !new PropertyOrFieldInfo(memberInfo).IsSettable)
				{
					iterator.FindPropertyRelative("value").boolValue = false;
					GUILayout.BeginHorizontal();
					GUI.enabled = false;
					EditorGUILayout.PropertyField(iterator, true);
					GUI.enabled = true;
					GUIStyle blueLabelStyle = new GUIStyle();
					blueLabelStyle.normal.textColor = new Color(0.2f, 0.6f, 1f);
					EditorGUILayout.LabelField(string.Format("Property {0} has no setter.", memberInfo.Name), blueLabelStyle);
					GUILayout.EndHorizontal();
					continue;
				}
				EditorGUILayout.PropertyField(iterator, true);
			}

			serializedObject.ApplyModifiedProperties();
		}

	}

}
