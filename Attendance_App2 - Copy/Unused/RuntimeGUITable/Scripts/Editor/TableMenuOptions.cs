using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor.UI;
using UnityEditor;
using UnityEngine.SceneManagement;
//using UnityEditor.Experimental.SceneManagement;

namespace UnityUITable
{

	static class TableMenuOptions
	{


		private const string kUILayerName = "UI";

		private const string kStandardSpritePath = "UI/Skin/UISprite.psd";
		private const string kBackgroundSpritePath = "UI/Skin/Background.psd";
		private const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
		private const string kKnobPath = "UI/Skin/Knob.psd";
		private const string kCheckmarkPath = "UI/Skin/Checkmark.psd";
		private const string kDropdownArrowPath = "UI/Skin/DropdownArrow.psd";
		private const string kMaskPath = "UI/Skin/UIMask.psd";

		static private UnityEngine.UI.DefaultControls.Resources s_StandardResources;

		static private UnityEngine.UI.DefaultControls.Resources GetStandardResources()
		{
			if (s_StandardResources.standard == null)
			{
				s_StandardResources.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
				s_StandardResources.background = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpritePath);
				s_StandardResources.inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>(kInputFieldBackgroundPath);
				s_StandardResources.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>(kKnobPath);
				s_StandardResources.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>(kCheckmarkPath);
				s_StandardResources.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>(kDropdownArrowPath);
				s_StandardResources.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>(kMaskPath);
			}
			return s_StandardResources;
		}

		private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
		{
			// Find the best scene view
			SceneView sceneView = SceneView.lastActiveSceneView;
			if (sceneView == null && SceneView.sceneViews.Count > 0)
				sceneView = SceneView.sceneViews[0] as SceneView;

			// Couldn't find a SceneView. Don't set position.
			if (sceneView == null || sceneView.camera == null)
				return;

			// Create world space Plane from canvas position.
			Vector2 localPlanePosition;
			Camera camera = sceneView.camera;
			Vector3 position = Vector3.zero;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
			{
				// Adjust for canvas pivot
				localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
				localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

				localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
				localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

				// Adjust for anchoring
				position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
				position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

				Vector3 minLocalPosition;
				minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
				minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;

				Vector3 maxLocalPosition;
				maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
				maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;

				position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
				position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
			}

			itemTransform.anchoredPosition = position;
			itemTransform.localRotation = Quaternion.identity;
			itemTransform.localScale = Vector3.one;
		}

		private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
		{
			GameObject parent = menuCommand.context as GameObject;
			if (parent == null || parent.GetComponentInParent<Canvas>() == null)
			{
				parent = GetOrCreateCanvasGameObject();
			}

			string uniqueName = GameObjectUtility.GetUniqueNameForSibling(parent.transform, element.name);
			element.name = uniqueName;
			Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
			Undo.SetTransformParent(element.transform, parent.transform, "Parent " + element.name);
			GameObjectUtility.SetParentAndAlign(element, parent);
			if (parent != menuCommand.context) // not a context click, so center in sceneview
				SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());

			Selection.activeGameObject = element;
		}

		// Controls

		[MenuItem("GameObject/UI/Table", false, 3000)]
		static public void AddTable(MenuCommand menuCommand)
		{
			GameObject go = DefaultControls.CreateTable(GetStandardResources());
			PlaceUIElementRoot(go, menuCommand);
		}

		// Helper methods

		static public GameObject CreateNewUI()
		{
			// Root for the UI
			var root = new GameObject("Canvas");
			root.layer = LayerMask.NameToLayer(kUILayerName);
			Canvas canvas = root.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			root.AddComponent<CanvasScaler>();
			root.AddComponent<GraphicRaycaster>();
			Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

			// if there is no event system add one...
			CreateEventSystem(false);
			return root;
		}

		private static void CreateEventSystem(bool select)
		{
			CreateEventSystem(select, null);
		}

		private static void CreateEventSystem(bool select, GameObject parent)
		{
			var esys = Object.FindObjectOfType<EventSystem>();
			if (esys == null)
			{
				var eventSystem = new GameObject("EventSystem");
				GameObjectUtility.SetParentAndAlign(eventSystem, parent);
				esys = eventSystem.AddComponent<EventSystem>();
				eventSystem.AddComponent<StandaloneInputModule>();

				Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
			}

			if (select && esys != null)
			{
				Selection.activeGameObject = esys.gameObject;
			}
		}

		// Helper function that returns a Canvas GameObject; preferably a parent of the selection, or other existing Canvas.
		static public GameObject GetOrCreateCanvasGameObject()
		{
			GameObject selectedGo = Selection.activeGameObject;

			// Try to find a gameobject that is the selected GO or one if its parents.
			Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
			if (canvas != null && canvas.gameObject.activeInHierarchy)
				return canvas.gameObject;

			// No canvas in selection or its parents? Then use just any canvas..
			canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
			if (canvas != null && canvas.gameObject.activeInHierarchy)
				return canvas.gameObject;

			// No canvas in the scene at all? Then create a new one.
			return CreateNewUI();
		}

	}

	public static class DefaultControls
	{
		private const float kWidth = 160f;
		private const float kThickHeight = 30f;
		private const float kThinHeight = 20f;
		private static Vector2 s_ThickElementSize = new Vector2(kWidth, kThickHeight);
		//private static Vector2 s_ThinElementSize = new Vector2(kWidth, kThinHeight);
		//private static Vector2 s_ImageElementSize = new Vector2(100f, 100f);
		//private static Color s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);
		//private static Color s_PanelColor = new Color(1f, 1f, 1f, 0.392f);
		private static Color s_TextColor = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);

		// Helper methods at top

		private static GameObject CreateUIElementRoot(string name, Vector2 size)
		{
			GameObject child = new GameObject(name);
			RectTransform rectTransform = child.AddComponent<RectTransform>();
			rectTransform.sizeDelta = size;
			return child;
		}

		static GameObject CreateUIObject(string name, GameObject parent)
		{
			GameObject go = new GameObject(name);
			go.AddComponent<RectTransform>();
			SetParentAndAlign(go, parent);
			return go;
		}

		private static void SetDefaultTextValues(Text lbl)
		{
			// Set text values we want across UI elements in default controls.
			// Don't set values which are the same as the default values for the Text component,
			// since there's no point in that, and it's good to keep them as consistent as possible.
			lbl.color = s_TextColor;

			// Reset() is not called when playing. We still want the default font to be assigned
			lbl.AssignDefaultFont();
		}

		private static void SetDefaultColorTransitionValues(Selectable slider)
		{
			ColorBlock colors = slider.colors;
			colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
			colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
			colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
		}

		private static void SetParentAndAlign(GameObject child, GameObject parent)
		{
			if (parent == null)
				return;

			child.transform.SetParent(parent.transform, false);
			SetLayerRecursively(child, parent.layer);
		}

		private static void SetLayerRecursively(GameObject go, int layer)
		{
			go.layer = layer;
			Transform t = go.transform;
			for (int i = 0; i < t.childCount; i++)
				SetLayerRecursively(t.GetChild(i).gameObject, layer);
		}

		// Moved internal helper from UnityEngine.UI.Text here

		private static void AssignDefaultFont(this Text lbl)
		{
			lbl.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		}

		public static GameObject CreateTable(UnityEngine.UI.DefaultControls.Resources resources)
		{
			GameObject buttonRoot = CreateUIElementRoot("Table", s_ThickElementSize);

			HorizontalLayoutGroup layoutGroup = buttonRoot.AddComponent<HorizontalLayoutGroup>();
			layoutGroup.childControlWidth = layoutGroup.childControlHeight = layoutGroup.childForceExpandWidth = layoutGroup.childForceExpandHeight = true;
			layoutGroup.spacing = -1f;

			ContentSizeFitter sizeFitter = buttonRoot.AddComponent<ContentSizeFitter>();
			sizeFitter.horizontalFit = sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			buttonRoot.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);

			buttonRoot.AddComponent<Table>();

			return buttonRoot;
		}

	}

}
