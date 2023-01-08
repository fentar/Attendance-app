using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUITable
{

	public static class CollectionUtils
	{

		public static void ForEach<T>(this T[] collection, Action<T> action)
		{
			foreach (T t in collection)
				action.Invoke(t);
		}

	}

}
