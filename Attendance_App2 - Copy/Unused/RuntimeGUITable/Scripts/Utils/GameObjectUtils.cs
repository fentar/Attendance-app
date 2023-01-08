using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityUITable
{

	public static class GameObjectUtils
	{

		public static Object InstantiatePrefab(Object original)
		{
			if (Application.isPlaying)
			{
				return Object.Instantiate(original);
			}
			else
			{
// InstantiatePrefab doesn't work with UI elements until 2018
#if UNITY_EDITOR && UNITY_2018_3_OR_NEWER
				return PrefabUtility.InstantiatePrefab(original);
#else
				return Object.Instantiate(original);
#endif
			}
		}

		public static Object InstantiatePrefab(Object original, Transform parent)
		{
			if (Application.isPlaying)
			{
				return Object.Instantiate(original, parent);
			}
			else
			{
#if UNITY_EDITOR && UNITY_2018_3_OR_NEWER
				Object instance = PrefabUtility.InstantiatePrefab(original);
				if (instance is Component)
				{
					(instance as Component).transform.SetParent(parent, false);
				}
				if (instance is GameObject)
				{
					(instance as GameObject).transform.SetParent(parent, false);
				}
				return instance;
#else
			return Object.Instantiate(original, parent);
#endif

			}
		}

		public static T InstantiatePrefab<T>(T original, Transform parent) where T : Object
		{
			if (Application.isPlaying)
			{
				return Object.Instantiate(original, parent);
			}
			else
			{
#if UNITY_EDITOR && UNITY_2018_3_OR_NEWER
				Object instance = PrefabUtility.InstantiatePrefab(original);

				if (instance is Component)
				{
					(instance as Component).transform.SetParent(parent, false);
				}
				if (instance is GameObject)
				{
					(instance as GameObject).transform.SetParent(parent, false);
				}
				return (T)instance;
#else
		return Object.Instantiate(original, parent);
#endif

			}
		
		}

	}

}
