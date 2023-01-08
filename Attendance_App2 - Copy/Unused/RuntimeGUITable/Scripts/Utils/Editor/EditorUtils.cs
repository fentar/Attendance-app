using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityUITable
{

	public static class EditorUtils
	{

		public static Rect MovePositionOneLineDown(this Rect prevPosition)
		{
			return new Rect(prevPosition.x, prevPosition.y + EditorGUIUtility.singleLineHeight + 2, prevPosition.width, EditorGUIUtility.singleLineHeight);
		}

		public static Rect SetPositionForFirstLine(this Rect prevPosition)
		{
			return new Rect(prevPosition.x, prevPosition.y + 2f, prevPosition.width, EditorGUIUtility.singleLineHeight);
		}
	}

}
