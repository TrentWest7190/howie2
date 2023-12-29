using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nexweron.Common.Attributes
{
	[CustomPropertyDrawer(typeof(InspectorAttribute), true)]
	internal sealed class InspectorDrawer : PropertyDrawer
	{
		private IEnumerable<InspectorAttribute> _inspectorAttrs;
		private PropertyAttribute _nativePropAttr;

		private bool _inited = false;
		private void Init(SerializedProperty field) {
			var attrs = fieldInfo.GetCustomAttributes(typeof(PropertyAttribute), true);
			var fieldAttrs = new List<PropertyAttribute>(attrs.Select(e => e as PropertyAttribute).OrderBy(e => e.order));
			_inspectorAttrs = fieldAttrs.OfType<InspectorAttribute>();
			_nativePropAttr = fieldAttrs.FirstOrDefault(NativePropertyDrawers.CheckNative);
			
			foreach (var multiAttr in _inspectorAttrs) {
				multiAttr.Init(field, fieldInfo);
			}
		}
		private void CheckInit(SerializedProperty field) {
			if (!_inited) {
				Init(field);
				_inited = true;
			}
		}
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			CheckInit(property);
			foreach (var multiAttr in _inspectorAttrs) {
				multiAttr.OnPreGUI(position, property, label);
			}
			
			bool isDrawn = false;
			foreach (var multiAttr in _inspectorAttrs) {
				isDrawn = multiAttr.OnGUI(position, property, label);
				if (isDrawn) { break; }
			}
			
			if (!isDrawn) {
				if (_nativePropAttr != null) {
					NativePropertyDrawers.OnGUI(_nativePropAttr, position, property, label);
				} else {
					EditorGUI.PropertyField(position, property, label);
				}
			}
			
			foreach (var multiAttr in _inspectorAttrs) {
				multiAttr.OnPostGUI(position, property, label);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			CheckInit(property);
			
			var height = base.GetPropertyHeight(property, label);
			foreach (var multiAttr in _inspectorAttrs) {
				height = multiAttr.GetPropertyHeight(property, label, height);
			}
			return height;
		}
	}

	public static class NativePropertyDrawers
	{
		internal static bool CheckNative(PropertyAttribute attribute) {
			return attribute is RangeAttribute || attribute is ColorUsageAttribute || attribute is DelayedAttribute;
		}

		internal static void OnGUI(PropertyAttribute attribute, Rect position, SerializedProperty property, GUIContent label) {
			if (attribute is RangeAttribute) {
				RangeDrawer.OnGUI(attribute, position, property, label);
			} else if (attribute is ColorUsageAttribute) {
				ColorUsageDrawer.OnGUI(attribute, position, property, label);
			} else if (attribute is DelayedAttribute) {
				DelayedDrawer.OnGUI(position, property, label);
			} else {
				EditorGUI.PropertyField(position, property, label);
			}
		}

		private static class RangeDrawer
		{
			internal static void OnGUI(PropertyAttribute attribute, Rect position, SerializedProperty property, GUIContent label) {
				RangeAttribute range = (RangeAttribute)attribute;
				if (property.propertyType == SerializedPropertyType.Float)
					EditorGUI.Slider(position, property, range.min, range.max, label);
				else if (property.propertyType == SerializedPropertyType.Integer)
					EditorGUI.IntSlider(position, property, (int)range.min, (int)range.max, label);
				else
					EditorGUI.LabelField(position, label.text, "Use Range with float or int.");
			}
		}

		private static class ColorUsageDrawer
		{
			internal static void OnGUI(PropertyAttribute attribute, Rect position, SerializedProperty property, GUIContent label) {
				var colorUsage = (ColorUsageAttribute)attribute;
				if (property.propertyType == SerializedPropertyType.Color) {
					label = EditorGUI.BeginProperty(position, label, property);
					EditorGUI.BeginChangeCheck();
					Color newColor = EditorGUI.ColorField(position, label, property.colorValue, true, colorUsage.showAlpha, colorUsage.hdr);
					if (EditorGUI.EndChangeCheck()) {
						property.colorValue = newColor;
					}
					EditorGUI.EndProperty();
				} else {
					EditorGUI.ColorField(position, label, property.colorValue, true, colorUsage.showAlpha, colorUsage.hdr);
				}
			}
		}

		private static class DelayedDrawer
		{
			public static void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
				if (property.propertyType == SerializedPropertyType.Float)
					EditorGUI.DelayedFloatField(position, property, label);
				else if (property.propertyType == SerializedPropertyType.Integer)
					EditorGUI.DelayedIntField(position, property, label);
				else if (property.propertyType == SerializedPropertyType.String)
					EditorGUI.DelayedTextField(position, property, label);
				else
					EditorGUI.LabelField(position, label.text, "Use Delayed with float, int, or string.");
			}
		}
	}
}