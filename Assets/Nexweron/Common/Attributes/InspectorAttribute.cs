using System;
using System.Reflection;
using UnityEngine;

namespace Nexweron.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
	public abstract class InspectorAttribute : PropertyAttribute
	{
		public InspectorAttribute() {
			order = int.MinValue;
		}

	#if UNITY_EDITOR
		public virtual void Init(UnityEditor.SerializedProperty field, FieldInfo fieldInfo) { }

		public virtual void OnPreGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label) { }
		public virtual bool OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label) { return false; }
		public virtual void OnPostGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label) { }
		public virtual float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label, float height) { return height; }
	#endif
	}
}
