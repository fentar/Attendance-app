using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityUITable
{

	public static class TransformUtils
	{

		public static void SetLocalX(this Transform root, float x)
		{
			Vector3 v3 = root.localPosition; v3.x = x;
			root.localPosition = v3;
		}
		public static void SetLocalY(this Transform root, float y)
		{
			Vector3 v3 = root.localPosition; v3.y = y;
			root.localPosition = v3;
		}
		public static void SetLocalZ(this Transform root, float z)
		{
			Vector3 v3 = root.localPosition; v3.z = z;
			root.localPosition = v3;
		}

		public static void SetWorldX(this Transform root, float x)
		{
			Vector3 v3 = root.position; v3.x = x;
			root.localPosition = v3;
		}
		public static void SetWorldY(this Transform root, float y)
		{
			Vector3 v3 = root.position; v3.y = y;
			root.localPosition = v3;
		}
		public static void SetWorldZ(this Transform root, float z)
		{
			Vector3 v3 = root.position; v3.z = z;
			root.localPosition = v3;
		}


		public static void DestroyChildren(this Transform root)
		{
			int childCount = root.childCount;
			for (int i = root.childCount - 1; i >= 0; i--)
			{
				GameObject.Destroy(root.GetChild(i).gameObject);
			}
		}

		public static void DestroyChildrenImmediate(this Transform root, params Transform[] exceptions)
		{
			List<Transform> excList = new List<Transform>(exceptions);
			int childCount = root.childCount;
			for (int i = root.childCount - 1; i >= 0; i--)
			{
				Transform t = root.GetChild(i);
				if (!excList.Contains(t))
					GameObject.DestroyImmediate(t.gameObject);
			}
		}

		public static List<Transform> FindChildren(this Transform root, string name)
		{
			List<Transform> res = new List<Transform>();
			for (int i = 0; i < root.childCount; i++)
			{
				Transform t = root.GetChild(i);
				if (t.name.Contains(name))
					res.Add(t);
				res.AddRange(t.FindChildren(name));
			}
			return res;
		}

		public static Transform Find(this Transform root, string name, bool recursive)
		{
			if (!recursive)
				return root.Find(name);
			Transform result = root.Find(name);
			if (result != null)
				return result;
			foreach (Transform child in root)
			{
				result = child.Find(name, true);
				if (result != null)
					return result;
			}
			return null;
		}

		public static List<Transform> GetChildrenList(this Transform root, bool recursive = false)
		{
			List<Transform> res = new List<Transform>();
			for (int i = 0; i < root.childCount; i++)
			{
				Transform t = root.GetChild(i);
				res.Add(t);
				if (recursive)
					res.AddRange(t.GetChildrenList(true));
			}
			return res;
		}

		public static List<GameObject> FindChildrenGameObjects(this Transform root, string name)
		{
			List<GameObject> res = new List<GameObject>();
			for (int i = 0; i < root.childCount; i++)
			{
				Transform t = root.GetChild(i);
				if (t.name.Contains(name))
					res.Add(t.gameObject);
				res.AddRange(t.FindChildrenGameObjects(name));
			}
			return res;
		}

		static public List<Transform> GetParents(this Transform trans, bool includingSelf = true)
		{
			if (trans == null) return null;
			List<Transform> res = new List<Transform>();
			if (includingSelf)
				res.Add(trans);
			if (trans.parent != null)
				res.AddRange(trans.parent.GetParents(true));
			return res;
		}

		/// <summary>
		/// Finds the specified component on the game object or one of its parents.
		/// </summary>

		static public T FindInParents<T>(this Transform trans) where T : Component
		{
			if (trans == null) return null;
			T comp = trans.GetComponent<T>();
			if (comp == null)
			{
				Transform t = trans.transform.parent;

				while (t != null && comp == null)
				{
					comp = t.gameObject.GetComponent<T>();
					t = t.parent;
				}
			}
			return comp;
		}

		public static Type GetOrAddComponent<Type>(this GameObject go) where Type : Component
		{
			Type res = go.GetComponent<Type>();
			if (res == null)
				res = go.AddComponent<Type>();
			return res;
		}

		public static Type GetOrAddComponent<Type>(this Component c) where Type : Component
		{
			Type res = c.GetComponent<Type>();
			if (res == null)
				res = c.gameObject.AddComponent<Type>();
			return res;
		}


		public static Type GetInParentsOrAddComponent<Type>(this GameObject go) where Type : Component
		{
			Type res = go.GetComponentInParent<Type>();
			if (res == null)
				res = go.AddComponent<Type>();
			return res;
		}

		public static Type GetInParentsOrAddComponent<Type>(this Component c) where Type : Component
		{
			Type res = c.GetComponentInParent<Type>();
			if (res == null)
				res = c.gameObject.AddComponent<Type>();
			return res;
		}

		public static void ResetAllLocalCoords(this Transform t)
		{
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
		}

		public static GameObject CreateChildGameObject(this Transform t)
		{
			GameObject go = new GameObject();
			go.transform.parent = t;
			go.transform.ResetAllLocalCoords();
			return go;
		}

		public static GameObject CreateChildGameObject(this Transform t, string name)
		{
			GameObject go = new GameObject(name);
			go.transform.parent = t;
			go.transform.ResetAllLocalCoords();
			return go;
		}

		public static GameObject CreateChildGameObject(this Transform t, string name, params System.Type[] components)
		{
			GameObject go = new GameObject(name, components);
			go.transform.parent = t;
			go.transform.ResetAllLocalCoords();
			return go;
		}

	}

}
