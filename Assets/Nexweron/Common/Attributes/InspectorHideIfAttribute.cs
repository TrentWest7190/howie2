using System;
using UnityEngine;

namespace Nexweron.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class InspectorShowIfAttribute : InspectorTargetIfAttribute
	{
		public InspectorShowIfAttribute(string targetName, object value) : base(targetName, value) { }
		protected InspectorShowIfAttribute(string targetName, object value, bool inverse) : base(targetName, value, inverse) { }
		public InspectorShowIfAttribute(string targetName, object v1, object v2) : base(targetName, v1, v2) { }
		protected InspectorShowIfAttribute(string targetName, object v1, object v2, bool inverse) : base(targetName, v1, v2, inverse) { }
		public InspectorShowIfAttribute(string targetName, object v1, object v2, object v3) : base(targetName, v1, v2, v3) { }
		protected InspectorShowIfAttribute(string targetName, object v1, object v2, object v3, bool inverse) : base(targetName, v1, v2, v3, inverse) { }
		
	#if UNITY_EDITOR
		public override bool OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label) {
			return _isValid && !isOn;
		}
		
		public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label, float height) {
			return (!_isValid || isOn) ? height : -2f;
		}
	#endif	
	}
	
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class InspectorHideIfAttribute : InspectorShowIfAttribute
	{
		public InspectorHideIfAttribute(string targetName, object value) : base(targetName, value, true) { }
		public InspectorHideIfAttribute(string targetName, object v1, object v2) : base(targetName, v1, v2, true) { }
		public InspectorHideIfAttribute(string targetName, object v1, object v2, object v3) : base(targetName, v1, v2, v3, true) { }
	}
}