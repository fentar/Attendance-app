using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityUITable
{

	public class TableStyleEditor : Editor
	{ 

		SerializedProperty rowsColorMode;
		SerializedProperty bgColor;
		SerializedProperty altBgColor;
		SerializedProperty lineColor;
		SerializedProperty titleBGColor;
		SerializedProperty titleFontColor;
		SerializedProperty titleFontSize;
		SerializedProperty rowHeight;
		SerializedProperty spacing;
		SerializedProperty outlineSprite;
		SerializedProperty selectableRows;
		SerializedProperty selectedBgColor;
		SerializedProperty cellContainerPrefab;
		SerializedProperty columnTitlePrefab;

		bool advancedExpanded;

		TableStyle tableStyle;

		void OnEnable()
		{
			tableStyle = (TableStyle)target;
			rowsColorMode 			= serializedObject.FindProperty("rowsColorMode");
			bgColor 				= serializedObject.FindProperty("bgColor");
			altBgColor 				= serializedObject.FindProperty("altBgColor");
			lineColor 				= serializedObject.FindProperty("lineColor");
			titleBGColor 			= serializedObject.FindProperty("titleBGColor");
			titleFontColor 			= serializedObject.FindProperty("titleFontColor");
			titleFontSize 			= serializedObject.FindProperty("titleFontSize");
			rowHeight 				= serializedObject.FindProperty("rowHeight");
			spacing 				= serializedObject.FindProperty("spacing"); 
			outlineSprite 			= serializedObject.FindProperty("outlineSprite");
			selectableRows 			= serializedObject.FindProperty("selectableRows");
			selectedBgColor 		= serializedObject.FindProperty("selectedBgColor");
			cellContainerPrefab 	= serializedObject.FindProperty("cellContainerPrefab");
			columnTitlePrefab 		= serializedObject.FindProperty("columnTitlePrefab");
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.PropertyField(rowsColorMode, new GUIContent("Row Colors"));
			EditorGUI.indentLevel++;
			if (tableStyle.rowsColorMode == TableStyle.RowsColorMode.Plain)
				EditorGUILayout.PropertyField(bgColor, new GUIContent("Color"));
			else
			{
				EditorGUILayout.PropertyField(bgColor, new GUIContent("Odd Rows"));
				EditorGUILayout.PropertyField(altBgColor, new GUIContent("Even Rows"));
			}
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(selectableRows, new GUIContent("Selectable Lines"));
			if (selectableRows.boolValue)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(selectedBgColor, new GUIContent("Selected Color"));
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.Space();
			bool oldOutline = lineColor.colorValue != Color.clear;
			bool newOutline = EditorGUILayout.Toggle(new GUIContent("Outline"), oldOutline);
			if (oldOutline && !newOutline)
			{
				lineColor.colorValue = Color.clear;
				spacing.floatValue = 0f;
			}
			else if (!oldOutline && newOutline)
			{
				lineColor.colorValue = Color.black;
				spacing.floatValue = -1f;
			}
			if (newOutline)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(lineColor, new GUIContent("Color"));
				EditorGUILayout.PropertyField(spacing);
				EditorGUI.indentLevel--;
				EditorGUILayout.Space();
			}
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Headers");
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(titleBGColor, new GUIContent("BG Color"));
			EditorGUILayout.PropertyField(titleFontColor, new GUIContent("Font Color"));
			EditorGUILayout.PropertyField(titleFontSize, new GUIContent("Font Size"));
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(rowHeight);
			EditorGUILayout.Space();

			advancedExpanded = EditorGUILayout.Foldout(advancedExpanded, "Advanced Settings");
			if (advancedExpanded)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(cellContainerPrefab);
				EditorGUILayout.PropertyField(columnTitlePrefab);
				EditorGUILayout.PropertyField(outlineSprite);
				EditorGUI.indentLevel--;
			}
		}

	}

}
