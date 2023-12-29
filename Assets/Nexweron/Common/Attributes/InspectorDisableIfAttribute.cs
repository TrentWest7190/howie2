using System;
using UnityEngine;

namespace Nexweron.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class InspectorDisableAttribute : InspectorIfAttribute
	{
		public InspectorDisableAttribute() : base(true) { }

	#if UNITY_EDITOR
		public override void OnPreGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label) {
			GUI.enabled = !_isValid || isOn;
		}

		public override void OnPostGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label) {
			GUI.enabled = true;
		}
	#endif	
	}
	
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class InspectorEnableIfAttribute : InspectorTargetIfAttribute
	{
		public InspectorEnableIfAttribute(string targetName, object value) : base(targetName, value) { }
		protected InspectorEnableIfAttribute(string targetName, object value, bool inverse) : base(targetName, value, inverse) { }
		public InspectorEnableIfAttribute(string targetName, object v1, object v2) : base(targetName, v1, v2) { }
		protected InspectorEnableIfAttribute(string targetName, object v1, object v2, bool inverse) : base(targetName, v1, v2, inverse) { }
		public InspectorEnableIfAttribute(string targetName, object v1, object v2, object v3) : base(targetName, v1, v2, v3) { }
		protected InspectorEnableIfAttribute(string targetName, object v1, object v2, object v3, bool inverse) : base(targetName, v1, v2, v3, inverse) { }
		
	#if UNITY_EDITOR
		public override void OnPreGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label) {
			GUI.enabled = !_isValid || isOn;
		}

		public override void OnPostGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label) {
			GUI.enabled = true;
		}
	#endif	
	}
	
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class InspectorDisableIfAttribute : InspectorEnableIfAttribute
	{
		public InspectorDisableIfAttribute(string targetName, object value) : base(targetName, value, true) { }
		public InspectorDisableIfAttribute(string targetName, object v1, object v2) : base(targetName, v1, v2, true) { }
		public InspectorDisableIfAttribute(string targetName, object v1, object v2, object v3) : base(targetName, v1, v2, v3, true) { }
	}
}