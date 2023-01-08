using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace UnityUITable
{

	[CustomPropertyDrawer(typeof(TableColumnInfo))]
	public class TableColumnInfoDrawer : PropertyDrawer
	{

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{

			EditorGUI.BeginProperty(position, label, property);
			TableColumnInfo tableColumnInfo = (TableColumnInfo)property.GetTargetObjectOfProperty();
			if (tableColumnInfo.elementType == null)
				return;

			TableEditor.OrientedWordsContainer orientedWords = tableColumnInfo.table == null ? TableEditor.VERTICAL_WORDS : TableEditor.GetOrientedWords(tableColumnInfo.table.horizontal);

			SerializedProperty fieldName = property.FindPropertyRelative("fieldName");
			SerializedProperty cellPrefab = property.FindPropertyRelative("cellPrefab");
			SerializedProperty width = property.FindPropertyRelative("width");
			SerializedProperty useRelativeWidth = property.FindPropertyRelative("useRelativeWidth");
			SerializedProperty autoWidth = property.FindPropertyRelative("autoWidth");
			SerializedProperty isSortable = property.FindPropertyRelative("isSortable");
			SerializedProperty expandableHeight = property.FindPropertyRelative("expandableHeight");
			SerializedProperty autoColumnTitle = property.FindPropertyRelative("autoColumnTitle");
			SerializedProperty columnTitle = property.FindPropertyRelative("columnTitle");
			SerializedProperty useColumnTitleImage = property.FindPropertyRelative("useColumnTitleImage");
			SerializedProperty columnTitleImage = property.FindPropertyRelative("columnTitleImage");
			SerializedProperty cellStyle = property.FindPropertyRelative("cellStyle");

			TableCell[] allCellPrefabs = AssetDatabase.FindAssets("t:Prefab")
					.Select(guid => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)))
					.Where(prefab => prefab.GetComponent<TableCell>() != null)
					.Select(prefab => prefab.GetComponent<TableCell>())
					.ToArray();

			position = position.SetPositionForFirstLine();

			MemberInfo[] members = tableColumnInfo.GetValidMembers();

			string[] propertyOptions = members
				.Select(pi => pi.Name)
				.ToArray();
			string[] propertyWithTypeOptions = members
				.Select(pi => (pi.MemberType == MemberTypes.Method) ? string.Format("{0} ()", pi.Name) : string.Format("{1} {0}", pi.Name, GetTypeName(new PropertyOrFieldInfo(pi).Type)))
				.ToArray();

			int selectedPropertyIndex = System.Array.IndexOf(propertyOptions, fieldName.stringValue);
			property.serializedObject.ApplyModifiedProperties();
			bool wasValid = tableColumnInfo.IsMemberValid;
			if (!wasValid)
			{
				if (string.IsNullOrEmpty(fieldName.stringValue))
					propertyWithTypeOptions = new string[] { "No Property" }.Concat(propertyWithTypeOptions).ToArray();
				else if (!tableColumnInfo.IsDefined)
					propertyWithTypeOptions = new string[] { string.Format("<Missing: {0}>", fieldName.stringValue) }.Concat(propertyWithTypeOptions).ToArray();
				selectedPropertyIndex = 0;
			}

			selectedPropertyIndex = EditorGUI.Popup(position, "Property", selectedPropertyIndex, propertyWithTypeOptions);

			if (!wasValid && selectedPropertyIndex <= 0)
			{
				position = position.MovePositionOneLineDown();
				EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, 1f), new Color(0.8f, 0.8f, 0.8f));
				return;
			}

			fieldName.stringValue = propertyOptions[!wasValid ? selectedPropertyIndex - 1 : selectedPropertyIndex];
			MemberInfo selectedMemberInfo = members[!wasValid ? selectedPropertyIndex - 1 : selectedPropertyIndex];

			TableCell[] compatibleCellPrefabs = allCellPrefabs
				.Where(cell => cell.IsCompatibleWithMember(selectedMemberInfo))
				.OrderByDescending(cell => cell.GetPriority(selectedMemberInfo))
				.ToArray();
			string[] cellOptions = compatibleCellPrefabs
				.Select(prefab => prefab.name)
				.ToArray();
			position = position.MovePositionOneLineDown();
			int selectedCellIndex = System.Array.IndexOf(compatibleCellPrefabs, cellPrefab.objectReferenceValue);
			if (selectedCellIndex < 0)
				selectedCellIndex = 0;
			selectedCellIndex = EditorGUI.Popup(new Rect(position.x, position.y, position.width - 24f, position.height), "Cell Type", selectedCellIndex, cellOptions);
			if (selectedCellIndex >= 0 && compatibleCellPrefabs.Count() > 0)
			{
				cellPrefab.objectReferenceValue = compatibleCellPrefabs[selectedCellIndex];
			}
			else
			{
				cellPrefab.objectReferenceValue = null;
			}
			if (cellPrefab.objectReferenceValue != null && cellPrefab.objectReferenceValue.GetType().BaseType.GetGenericArguments().Any())
			{
				string filename = string.Format("{0}_{1}_{2}_Style",
					(tableColumnInfo.table != null ? tableColumnInfo.table.name : "UndefinedTable"),
					tableColumnInfo.fieldName,
					cellPrefab.objectReferenceValue.name.Replace(" ", ""));
				DrawCellStyleButton(
					new Rect(position.x + position.width - 20f, position.y, 20f, position.height),
					cellStyle,
					(TableCell)cellPrefab.objectReferenceValue,
					filename,
					tableColumnInfo.table,
					null,
					selectedMemberInfo);
			}

			position = position.MovePositionOneLineDown();
			if (useColumnTitleImage.boolValue)
			{
				EditorGUI.PropertyField(new Rect(position.x, position.y, position.width - 24f, position.height), columnTitleImage, new GUIContent("Title"));
			}
			else
			{
				if (autoColumnTitle.boolValue)
					GUI.enabled = false;
				EditorGUI.PropertyField(new Rect(position.x, position.y, position.width - 64f, position.height), columnTitle, new GUIContent("Title"));
				GUI.enabled = true;
				EditorGUI.PropertyField(new Rect(position.x + position.width - 62f, position.y, 40f, position.height), autoColumnTitle, new GUIContent(""));
				EditorGUI.LabelField(new Rect(position.x + position.width - 48f, position.y, 50f, position.height), "auto");
			}
			if (GUI.Button(
				new Rect(position.x + position.width - 20f, position.y, 20f, position.height),
				useColumnTitleImage.boolValue ? EditorGUIUtility.IconContent("Texture Icon", "Use Image Title") : EditorGUIUtility.IconContent("TextMesh Icon", "Use Text Title")))
			{
				useColumnTitleImage.boolValue = !useColumnTitleImage.boolValue;
			}

			position = position.MovePositionOneLineDown();
			if (autoWidth.boolValue)
				GUI.enabled = false;
			EditorGUI.PropertyField(new Rect(position.x, position.y, position.width - 95f, position.height), width, new GUIContent(orientedWords.Width));
			GUI.enabled = true;
			bool oldUseRelativeValue = useRelativeWidth.boolValue;
			EditorGUI.PropertyField(new Rect(position.max.x - 93f, position.y, 60f, position.height), useRelativeWidth, new GUIContent(""));
			EditorGUI.LabelField(new Rect(position.max.x - 81f, position.y, 60f, position.height), "relative");
			EditorGUI.PropertyField(new Rect(position.max.x - 38f, position.y, 60f, position.height), autoWidth, new GUIContent(""));
			EditorGUI.LabelField(new Rect(position.max.x - 26f, position.y, 60f, position.height), "auto");


			if (useRelativeWidth.boolValue != oldUseRelativeValue && tableColumnInfo.table != null)
			{
				float totalWidth = tableColumnInfo.table.GetAvailableWidth();
				if (useRelativeWidth.boolValue && !oldUseRelativeValue)
					width.floatValue = width.floatValue / totalWidth;
				else if (!useRelativeWidth.boolValue && oldUseRelativeValue)
					width.floatValue = width.floatValue * totalWidth;
			}

			position = position.MovePositionOneLineDown();
			EditorGUI.PropertyField(position, isSortable, new GUIContent("Sortable"));
			position = position.MovePositionOneLineDown();
			EditorGUI.PropertyField(position, expandableHeight, new GUIContent("Expandable"));

			if (autoColumnTitle.boolValue)
			{
				columnTitle.stringValue = ObjectNames.NicifyVariableName(fieldName.stringValue);
			}

			property.serializedObject.ApplyModifiedProperties();
			position = position.MovePositionOneLineDown();
			EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, 1f), new Color(0.8f, 0.8f, 0.8f));
			EditorGUI.EndProperty();
		}


		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			TableColumnInfo tableColumnInfo = (TableColumnInfo)property.GetTargetObjectOfProperty();
			if (tableColumnInfo.elementType == null)
				return 0f;
			if (!tableColumnInfo.IsDefined)
				return 1.5f * EditorGUIUtility.singleLineHeight;
			return 7f * EditorGUIUtility.singleLineHeight;
		} 

		public static void DrawCellStyleButton(Rect position, SerializedProperty cellStyle, TableCell cellPrefab, string filename, Table table, ScriptableObject template = null, MemberInfo memberInfo = null)
		{
			EditorGUIUtility.SetIconSize(12f * Vector2.one);
			bool changed = GUI.changed;
			if (GUI.Button(new Rect(position.x + position.width - 20f, position.y, 20f, position.height), EditorGUIUtility.IconContent("ClothInspector.SettingsTool")))
			{
				CellStyleWindow.CreateStyleWindow(cellStyle, table, memberInfo, cellPrefab, template, filename);
			}
			GUI.changed = changed;
			EditorGUIUtility.SetIconSize(Vector2.zero);
		}

		private static string GetTypeName(System.Type t)
		{
			if (t == typeof(int))
				return "int";
			if (t == typeof(float))
				return "float";
			if (t == typeof(string))
				return "string";
			if (t == typeof(bool))
				return "bool";
			return t.Name;
		}

	}

}

