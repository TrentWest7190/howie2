using System;
using UnityEngine;

namespace Nexweron.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class InspectorIndentAttribute : InspectorAttribute
	{
		private int _indent = 0;
		
		public InspectorIndentAttribute(int indent) : base() {
			_indent = indent;
		}
		
	#if UNITY_EDITOR
		private int _indentPrev = 0;
		
		public override void OnPreGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label) {
			_indentPrev = UnityEditor.EditorGUI.indentLevel;
			UnityEditor.EditorGUI.indentLevel = _indent;
		}

		public override void OnPostGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label) {
			UnityEditor.EditorGUI.indentLevel = _indentPrev;
		}
	#endif
	}
}
