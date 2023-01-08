using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityUITable
{

	[ExecuteInEditMode]
	public class Table : MonoBehaviour
	{

		public enum State { UndefinedState, InvalidCollection, InvalidColumns, Valid }
		[SerializeField][HideInInspector] State currentState = State.UndefinedState;

		bool isDirty;

		public void SetDirty()
		{
			isDirty = true;
		}

		public static bool IsCollection(System.Type type)
		{
			if (type == null || type == typeof(string) || type == typeof(Transform))
				return false;
			return typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType;
		}

		static bool IsCollection(MemberInfo memberInfo)
		{
			return memberInfo.MemberType == MemberTypes.Field && IsCollection(((FieldInfo)memberInfo).FieldType)
				|| memberInfo.MemberType == MemberTypes.Property && IsCollection(((PropertyInfo)memberInfo).PropertyType);
		}

		public static readonly Color DARK_BLUE = new Color(0f, 0f, 0.5f);
		public static readonly Color DARK_GRAY = new Color(0.4f, 0.4f, 0.4f);
		public static readonly Color SELECTION_BLUE = new Color(0.2f, 0.4f, 0.5f);

		public TableCellContainer cellContainerPrefab;
		public CellContainer emptyCellContainerPrefab;
		public CellContainer addButtonCellContainerPrefab;

		public HeaderCellContainer columnTitlePrefab;

		public List<TableColumnInfo> columns = new List<TableColumnInfo>();

		public bool rowDeleteButtons;

		public float deleteColumnWidth = 40f;

		public ButtonCell deleteCellPrefab;

		public ButtonCellStyle deleteCellStyle;
		public ButtonCellStyle deleteCellStyleTemplate;
		public ButtonCellStyle DeleteCellStyle { get { return (deleteCellStyle != null) ? deleteCellStyle : deleteCellStyleTemplate; } }
		public DeleteColumnInfo deleteColumnInfo { get { return new DeleteColumnInfo(this); } }

		public bool rowAddButton;

		public ButtonCell addCellPrefab;

		public ButtonCellStyle addCellStyle;
		public ButtonCellStyle addCellStyleTemplate;
		public ButtonCellStyle AddCellStyle { get { return (addCellStyle != null) ? addCellStyle : addCellStyleTemplate; } }
		public AddCellInfo addCellInfo { get { return new AddCellInfo(this); } }

		public enum RowsColorMode { Plain, Striped }
		public RowsColorMode rowsColorMode;
		public Color bgColor = Color.gray;
		public Color altBgColor = DARK_GRAY;

		public bool selectableRows;
		public Color selectedBgColor = SELECTION_BLUE;

		public Color lineColor = Color.white;
		public bool hasTitles = true;
		public Color titleBGColor = DARK_BLUE;
		public Color titleFontColor = Color.white;
		public int titleFontSize = 14;
		public Font titleFont;
		public FontStyle titleFontStyle;

		public float rowHeight = 32f;
		public float spacing = -1f;

		public Sprite outlineSprite;

		public bool updateCellStyleAtRuntime;
		public bool updateCellContentAtRuntime;

		public bool limitRowsInEditMode = true;
		public int nbRowsInEditMode = 10;
		public bool preinstantiateRowsOverLimit = false;

		public bool horizontal = false;

		public UnityMemberInfo targetCollection = new UnityMemberInfo(IsCollection);

		SortingState _sortingState = new SortingState();
		public SortingState sortingState { get { return _sortingState; } private set { _sortingState = value; } }

		List<float> heights = new List<float>();
		public float GetHeight(int rowIndex)
		{
			if (rowIndex == 0 || rowIndex - 1 >= heights.Count)
				return rowHeight;
			return heights[rowIndex - 1];
		}

		int selectedRow = -1;

		public int SelectedRow {  get { return selectedRow; } }

		[SerializeField] [HideInInspector] List<TableColumn> tableColumns = new List<TableColumn>();

		#region CachedElements

		List<TableColumnInfo> validColumns = new List<TableColumnInfo>();

		void UpdateValidColumns()
		{
			validColumns = columns.Where(c => c.IsDefined).ToList();
		}

		#endregion

		RectTransform _rectTransform;
		RectTransform rectTransform
		{
			get
			{
				if (_rectTransform == null)
					_rectTransform = GetComponent<RectTransform>();
				return _rectTransform;
			}
		}

		public IEnumerable<object> GetCollectionElements()
		{
			if (!Application.isPlaying && limitRowsInEditMode && !preinstantiateRowsOverLimit && targetCollection.Collection.Count > nbRowsInEditMode)
				return targetCollection.Collection.Cast<object>().Take(nbRowsInEditMode).ToList();	
			return targetCollection.Collection.Cast<object>().ToList();
		}

		public int ElementCount
		{
			get
			{
				if (targetCollection.Collection == null) return 0;
				if (!Application.isPlaying && limitRowsInEditMode && !preinstantiateRowsOverLimit)
					return Mathf.Min(targetCollection.Collection.Count, nbRowsInEditMode);
				return targetCollection.Collection.Count;
			}
		}

		public System.Type ElementType
		{
			get
			{
				if (!targetCollection.IsDefined)
					return null;
				return targetCollection.ElementType;
			}
		}

		public float GetAvailableWidth(int nbColumns)
		{
			Rect rect = rectTransform.rect;
			float total = horizontal ? rect.height : rect.width;
			return total - (rowDeleteButtons ? deleteColumnWidth : 0f) - spacing * (nbColumns - 1 + (rowDeleteButtons ? 1 : 0));
		}

		public float GetAvailableWidth()
		{
			return GetAvailableWidth(validColumns.Count);
		}

		public TableColumnInfo GetColumnInfoAt(int index)
		{
			if (index == validColumns.Count && rowDeleteButtons)
				return deleteColumnInfo;
			else if (index < 0)
				return addCellInfo;
			else
				return validColumns[index];
		}

		HorizontalOrVerticalLayoutGroup _hGroup;
		HorizontalOrVerticalLayoutGroup hGroup
		{
			get
			{
				if (_hGroup == null) _hGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
				return _hGroup;
			}
		}

		ContentSizeFitter _fitter;
		ContentSizeFitter fitter
		{
			get
			{
				if (_fitter == null) _fitter = GetComponent<ContentSizeFitter>();
				return _fitter;
			}
		}

		public Color GetRowBGColor(int rowIndex)
		{
			if (selectableRows && rowIndex == selectedRow)
				return selectedBgColor;
			if (rowsColorMode == RowsColorMode.Plain)
				return bgColor;
			else
				return rowIndex % 2 == 0 ? altBgColor : bgColor;
		}

		private void Start()
		{
			UpdateValidColumns();
			sortingState = new SortingState();
			UpdateContent();
		}

		void HandleInputs()
		{
			if (Input.GetMouseButton(0))
				selectedRow = -1;
			int elementCount = ElementCount;
			if (selectedRow >= 0 && selectedRow < elementCount)
			{
				if (Input.GetKeyUp(KeyCode.UpArrow))
					selectedRow = Mathf.Max(selectedRow - 1, 1);
				else if (Input.GetKeyUp(KeyCode.DownArrow))
					selectedRow = Mathf.Min(selectedRow + 1, elementCount);
			}
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				HandleTabKey(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
			}
		}

		enum TabDirection { Forward, Backward }

		private void HandleTabKey(bool isNavigateBackward)
		{
			GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
			if (selectedObject != null && selectedObject.activeInHierarchy)
			{
				Selectable currentSelection = selectedObject.GetComponent<Selectable>();
				if (currentSelection != null)
				{
					if (currentSelection.GetComponentInParent<Table>() != this)
						return;
					Selectable nextSelection =  FindNextSelectable(currentSelection, isNavigateBackward ? TabDirection.Backward: TabDirection.Forward);
					if (nextSelection != null)
					{
						nextSelection.Select();
					}
				}
			}
		}

		Selectable FindNextSelectable(Selectable selectable, TabDirection direction)
		{
			int columnIndex = -1, rowIndex = - 1;
			CellContainer cellContainer = selectable.GetComponentInParent<CellContainer>();
			for (int i = 0; i < tableColumns.Count; i++)
			{
				int row = tableColumns[i].IndexOf(cellContainer);
				if (row >= 0)
				{
					columnIndex = i;
					rowIndex = row;
					break;
				}
			}
			if (columnIndex < 0 || rowIndex < 0)
				return null;
			if (direction == TabDirection.Forward)
			{
				for (int i = rowIndex; i < ElementCount; i++)
				{
					int startIndex = (i == rowIndex) ? columnIndex + 1 : 0;
					for (int j = startIndex; j < tableColumns.Count; j++)
					{
						Selectable s = GetSelectableFromCellAt(j, i);
						if (s != null)
							return s;
					}
				}
			}
			else
			{
				for (int i = rowIndex; i >= 0; i--)
				{
					int startIndex = (i == rowIndex) ? columnIndex - 1 : tableColumns.Count - 1;
					for (int j = startIndex; j >= 0; j--)
					{
						Selectable s = GetSelectableFromCellAt(j, i);
						if (s != null)
							return s;
					}
				}
			}
			return null;
		}

		Selectable GetSelectableFromCellAt(int columnIndex, int rowIndex)
		{
			TableColumn column = tableColumns[columnIndex];
			CellContainer cc = column.GetCellAt(rowIndex);
			return (cc == null) ? null : cc.GetComponentInChildren<Selectable>();
		}

		void UpdateColumnWidths()
		{
			float totalWidth = GetAvailableWidth(validColumns.Count);
			System.Func<TableColumnInfo, float> GetAbsoluteWidth = (c) => c.useRelativeWidth ? c.width * totalWidth : c.width;
			foreach (TableColumnInfo column in validColumns)
			{
				if (column.autoWidth)
				{
					float newWidth = (totalWidth - validColumns.Where(c => !c.autoWidth).Sum(c => GetAbsoluteWidth(c))) / validColumns.Count(c => c.autoWidth);
					if (column.useRelativeWidth)
						newWidth = newWidth / totalWidth;
					if (!Mathf.Approximately(newWidth, column.width))
					{
						column.width = newWidth;
					}
				}
				column.AbsoluteWidth = column.useRelativeWidth ? column.width * totalWidth : column.width;
			}
		}

		void UpdateRowHeights()
		{
			if (heights.Count > ElementCount)
				heights.RemoveRange(0, heights.Count - ElementCount);
			else if (heights.Count < ElementCount)
				heights.AddRange(Enumerable.Repeat<float>(0, ElementCount - heights.Count));
			if (validColumns.All(vc => !vc.expandableHeight))
			{
				for (int i = 0; i < heights.Count; i++)
					heights[i] = rowHeight;
			}
			else
			{
				for (int i = 0; i < heights.Count; i++)
					heights[i] = Mathf.Max(rowHeight, tableColumns.Max(tc => tc.GetCellAt(i)?.contentRequiredHeight ?? 0f));
			}
		}

		private void Update()
		{
			Canvas.ForceUpdateCanvases();

			UpdateValidColumns();

			if (rectTransform.sizeDelta.x == 0f && rectTransform.anchorMin.x == rectTransform.anchorMax.x)
				rectTransform.sizeDelta = new Vector2(200f, 100f);
			HandleInputs();

			bool unconstrained = (validColumns.Count == 0 || validColumns.Any(c => c.autoWidth || c.useRelativeWidth));
			ContentSizeFitter.FitMode fitMode = unconstrained ? ContentSizeFitter.FitMode.Unconstrained : ContentSizeFitter.FitMode.PreferredSize;

			if (horizontal)
				fitter.verticalFit = fitMode;
			else
				fitter.horizontalFit = fitMode;

			UpdateState();

			UpdateColumnWidths();
			UpdateRowHeights();

			if (horizontal != (hGroup is VerticalLayoutGroup))
			{
				SetDirty();
				DestroyImmediate(hGroup);
				gameObject.AddComponent(horizontal ? typeof(VerticalLayoutGroup): typeof(HorizontalLayoutGroup));
				hGroup.childControlWidth = hGroup.childControlHeight = hGroup.childForceExpandWidth = hGroup.childForceExpandHeight = true;
				hGroup.spacing = -1f;
			}

			if (currentState == State.Valid && transform.childCount != validColumns.Count() + (rowDeleteButtons ? 1 : 0))
			SetDirty();

			if (isDirty)
			{
				Debug.Log("Reinitialize");
				Initialize();
			}
			//else
			{
				if (updateCellContentAtRuntime || !Application.isPlaying)
					UpdateContent();
				if (updateCellStyleAtRuntime || !Application.isPlaying)
					UpdateStyle();
			}

			hGroup.spacing = spacing;

		}

		[ContextMenu("Initialize")]
		public void Initialize()
		{
			Debug.Log("Initialize");
			while (transform.childCount > 0)
				DestroyImmediate(transform.GetChild(0).gameObject);
			tableColumns.Clear();
			if (!targetCollection.IsDefined)
			{
				GameObject content = transform.CreateChildGameObject("Text");
				Text text = content.AddComponent<Text>();
				text.text = "Table - Target not defined.";
				text.alignment = TextAnchor.MiddleCenter;
				isDirty = false;
				return;
			}
			foreach (TableColumnInfo column in columns)
			{
				column.table = this;
			}
			UpdateValidColumns();
			if (validColumns.Count == 0)
			{
				GameObject content = transform.CreateChildGameObject("Text");
				Text text = content.AddComponent<Text>();
				text.text = "Table - No valid columns.";
				text.alignment = TextAnchor.MiddleCenter;
				isDirty = false;
				return;
			}
			int columnIndex = 0;
			foreach (TableColumnInfo column in validColumns)
			{
				TableColumn tableColumn = CreateColumn<TableColumn>("Column_" + column.fieldName);
				tableColumn.Initialize(columnIndex);
				tableColumns.Add(tableColumn);
				columnIndex++;
			}
			if (rowDeleteButtons)
			{
				TableColumn tableColumn = CreateColumn<DeleteColumn>("Column_Delete");
				tableColumn.Initialize(columnIndex);
				tableColumns.Add(tableColumn);
				columnIndex++;
			}
			isDirty = false;
		}

		TableColumn CreateColumn<ColumnType>(string goName) where ColumnType : TableColumn
		{
			GameObject columnGO = transform.CreateChildGameObject(goName);
			columnGO.transform.parent = transform;
			TableColumn tableColumn = columnGO.AddComponent<ColumnType>();
			return tableColumn;
		}

		List<object> _sortedElements;
		public List<object> GetSortedElements()
		{
			if (_sortedElements == null)
				UpdateSortedElements();
			return _sortedElements;
		}
		void UpdateSortedElements()
		{
			if (sortingState.sortMode == SortingState.SortMode.Ascending)
				_sortedElements = GetCollectionElements().OrderBy(sortingState.KeySelector).ToList();
			else if (sortingState.sortMode == SortingState.SortMode.Descending)
				_sortedElements = GetCollectionElements().OrderByDescending(sortingState.KeySelector).ToList();
			else
				_sortedElements = GetCollectionElements().ToList();
		}

		public void ColumnTitleClicked(TableColumnInfo column)
		{
			sortingState.ClickOnColumn(column);
			UpdateContent();
		}

		public void SetSelected(int rowIndex)
		{
			selectedRow = rowIndex;
		}

		public void UpdateContent()
		{
			if (currentState != State.Valid)
				return;
			targetCollection.UpdateCache();
			UpdateSortedElements();
			foreach (TableColumn column in tableColumns)
				column.UpdateContent();
		}

		public void UpdateStyle()
		{
			foreach (TableColumn column in tableColumns)
				column.UpdateStyle();
		}

		void UpdateState()
		{
			State newState = GetState();
			if (newState != currentState)
			{
				SetDirty();
				currentState = newState;
			}
		}

		State GetState()
		{
			if (!targetCollection.IsDefined)
				return State.InvalidCollection;
			if (validColumns.Count == 0)
				return State.InvalidColumns;
			return State.Valid;
		}

#if UNITY_EDITOR

		public ScrollRect scrollViewPrefab;

		public bool IsScrollable
		{
			get
			{
				return transform.parent != null && transform.parent.parent != null && transform.parent.parent.GetComponent<ScrollRect>() != null;
			}
		}

		public void MakeScrollable(bool scrollable)
		{
			if (scrollable)
			{
				if (IsScrollable)
					return;
				this.GetComponent<ContentSizeFitter>().enabled = false;
				ScrollRect scrollView = (ScrollRect)GameObjectUtils.InstantiatePrefab(scrollViewPrefab);
				scrollView.transform.SetParent(this.transform.parent, false);
				RectTransform tableRT = GetComponent<RectTransform>();
				RectTransform scrollViewRT = scrollView.GetComponent<RectTransform>();
				UnityEditorInternal.ComponentUtility.CopyComponent(tableRT);
				UnityEditorInternal.ComponentUtility.PasteComponentValues(scrollViewRT);
				tableRT.SetParent(scrollView.viewport, false);
				tableRT.sizeDelta = Vector2.zero;
				scrollView.content = tableRT;
				this.GetComponent<ContentSizeFitter>().enabled = true;
				EditorGUIUtility.PingObject(gameObject);
			}
			else
			{
				if (!IsScrollable)
					return;
				this.GetComponent<ContentSizeFitter>().enabled = false;
				ScrollRect scrollView = this.transform.parent.parent.GetComponent<ScrollRect>();
				RectTransform tableRT = GetComponent<RectTransform>();
				RectTransform scrollViewRT = scrollView.GetComponent<RectTransform>();
				tableRT.parent = scrollViewRT.parent;
				UnityEditorInternal.ComponentUtility.CopyComponent(scrollViewRT);
				UnityEditorInternal.ComponentUtility.PasteComponentValues(tableRT);
				DestroyImmediate(scrollView.gameObject);
				this.GetComponent<ContentSizeFitter>().enabled = true;
				EditorGUIUtility.PingObject(gameObject);
			}
		}

#endif

	}

}