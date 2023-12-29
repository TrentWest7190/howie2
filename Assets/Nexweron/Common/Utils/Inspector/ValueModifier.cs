#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;

namespace Nexweron.Common.Utils
{
	public abstract class ValueModifier
	{
		protected bool _isValid = false;
		public bool isValid {
			get { return _isValid; }
		}

		protected Func<object> _valueGetter = () => null;
		public object GetValue() {
			return _valueGetter();
		}
		
		protected Action<object> _valueSetter = (value) => { };
		public void SetValue(object value) {
			_valueSetter(value);
		}
	}
	
	public class SerializedFieldModifier : ValueModifier
	{
		private UnityEditor.SerializedProperty _field;
		public SerializedFieldModifier(UnityEditor.SerializedProperty field) {
			bool isValid = false;
			_field = field;
			if (field != null) {
				isValid = true;
				var targetObject = field.serializedObject.targetObject;
				Debug.Log($"field.type: {field.type}");
				switch (field.propertyType) {
					//case SerializedPropertyType.Generic:
					case UnityEditor.SerializedPropertyType.Integer:
						_valueGetter = () => field.intValue;
						_valueSetter = (value) => { field.intValue = (int)value; };
						break;
					case UnityEditor.SerializedPropertyType.Boolean:
						_valueGetter = () => field.boolValue;
						_valueSetter = (value) => { field.boolValue = (bool)value; };
						break;
					case UnityEditor.SerializedPropertyType.Float:
						_valueGetter = () => field.floatValue;
						_valueSetter = (value) => { field.floatValue = (float)value; };
						break;
					case UnityEditor.SerializedPropertyType.String:
						_valueGetter = () => field.stringValue;
						_valueSetter = (value) => { field.stringValue = (string)value; };
						break;
					case UnityEditor.SerializedPropertyType.Color:
						_valueGetter = () => field.colorValue;
						_valueSetter = (value) => { field.colorValue = (Color)value; };
						break;
					case UnityEditor.SerializedPropertyType.ObjectReference:
						_valueGetter = () => field.objectReferenceValue;
						_valueSetter = (value) => { field.objectReferenceValue = (UnityEngine.Object)value; };
						break;
					case UnityEditor.SerializedPropertyType.LayerMask:
						_valueGetter = () => field.intValue;
						_valueSetter = (value) => { field.intValue = (int)value; };
						break;
					case UnityEditor.SerializedPropertyType.Enum:
						_valueGetter = () => field.enumValueIndex;
						_valueSetter = (value) => { field.enumValueIndex = (int)value; };
						break;
					case UnityEditor.SerializedPropertyType.Vector2:
						_valueGetter = () => field.vector2Value;
						_valueSetter = (value) => { field.vector2Value = (Vector2)value; };
						break;
					case UnityEditor.SerializedPropertyType.Vector3:
						_valueGetter = () => field.vector3Value;
						_valueSetter = (value) => { field.vector3Value = (Vector3)value; };
						break;
					case UnityEditor.SerializedPropertyType.Vector4:
						_valueGetter = () => field.vector4Value;
						_valueSetter = (value) => { field.vector4Value = (Vector4)value; };
						break;
					case UnityEditor.SerializedPropertyType.Rect:
						_valueGetter = () => field.rectValue;
						_valueSetter = (value) => { field.rectValue = (Rect)value; };
						break;
					//case SerializedPropertyType.ArraySize:
					case UnityEditor.SerializedPropertyType.Character:
						_valueGetter = () => (Char)field.intValue;
						_valueSetter = (value) => { field.intValue = (Char)value; };
						break;
					case UnityEditor.SerializedPropertyType.AnimationCurve:
						_valueGetter = () => field.animationCurveValue;
						_valueSetter = (value) => { field.animationCurveValue = (AnimationCurve)value; };
						break;
					case UnityEditor.SerializedPropertyType.Bounds:
						_valueGetter = () => field.boundsValue;
						_valueSetter = (value) => { field.boundsValue = (Bounds)value; };
						break;
					//case SerializedPropertyType.Gradient:
					case UnityEditor.SerializedPropertyType.Quaternion:
						_valueGetter = () => field.quaternionValue;
						_valueSetter = (value) => { field.quaternionValue = (Quaternion)value; };
						break;
					//case SerializedPropertyType.ExposedReference:
					//case SerializedPropertyType.FixedBufferSize:
					case UnityEditor.SerializedPropertyType.Vector2Int:
						_valueGetter = () => field.vector2IntValue;
						_valueSetter = (value) => { field.vector2IntValue = (Vector2Int)value; };
						break;
					case UnityEditor.SerializedPropertyType.Vector3Int:
						_valueGetter = () => field.vector3IntValue;
						_valueSetter = (value) => { field.vector3IntValue = (Vector3Int)value; };
						break;
					case UnityEditor.SerializedPropertyType.RectInt:
						_valueGetter = () => field.rectIntValue;
						_valueSetter = (value) => { field.rectIntValue = (RectInt)value; };
						break;
					case UnityEditor.SerializedPropertyType.BoundsInt:
						_valueGetter = () => field.boundsIntValue;
						_valueSetter = (value) => { field.boundsIntValue = (BoundsInt)value; };
						break;
					default:
						isValid = false;
						break;
				}
			} else {
				Debug.LogError("field = null");
			}
			_isValid = isValid;
		}
	}
	
	public class FieldModifier : ValueModifier
	{
		private FieldInfo _info;
		public FieldInfo info => _info;

		public FieldModifier(object targetObject, string fieldName) {
			var defaultPath = fieldName;
			
			bool isValid = false;
			_info = ReflectionUtils.FindFieldByPath(ref targetObject, defaultPath);
			
			if (_info != null) {
				isValid = true;
				var T = _info.FieldType;
				_valueGetter = () => _info.GetValue(targetObject);
				_valueSetter = (value) => { _info.SetValue(targetObject, value); };
				
				if (_info.FieldType == typeof(LayerMask)) {
					_valueGetter = () => ((LayerMask)_info.GetValue(targetObject)).value;
					_valueSetter = (value) => {
						var lm = (LayerMask)_info.GetValue(targetObject);
						lm.value = (int)value;
						_info.SetValue(targetObject, lm);
					};
				} else
				/*if (_info.FieldType.IsEnum) {
					_valueGetter = () => _info.GetValue(targetObject); // (int)
				} else*/
				if (_info.FieldType == typeof(Char)) {
					_valueGetter = () => (Char)_info.GetValue(targetObject);
				} else
				if (_info.FieldType == typeof(int) ||
					_info.FieldType == typeof(bool) ||
					_info.FieldType == typeof(float) ||
					_info.FieldType == typeof(string) ||
					_info.FieldType == typeof(Color) ||
					_info.FieldType == typeof(Vector2) ||
					_info.FieldType == typeof(Vector3) ||
					_info.FieldType == typeof(Vector4) ||
					_info.FieldType == typeof(Rect) ||
					_info.FieldType == typeof(AnimationCurve) ||
					_info.FieldType == typeof(Bounds) ||
					_info.FieldType == typeof(Quaternion) ||
					_info.FieldType == typeof(Vector2Int) ||
					_info.FieldType == typeof(Vector3Int) ||
					_info.FieldType == typeof(RectInt) ||
					_info.FieldType == typeof(BoundsInt) ||
					_info.FieldType.IsEnum ||
					_info.FieldType.IsSubclassOf(typeof(UnityEngine.Object))
					) {
				} else {
					isValid = false;
					Debug.LogError(fieldName + " field type " + _info.FieldType + " is not a valid type");
				}
			} else {
				//Debug.LogError("Can't find field " + defaultPath);
			}
			_isValid = isValid;
		}
	}
	
	public class PropertyModifier : ValueModifier
	{
		private PropertyInfo _info;
		public PropertyInfo info => _info;

		public PropertyModifier(object targetObject, string propName) {
			var defaultPath = propName;
			
			bool isValid = false;
			_info = ReflectionUtils.FindPropByPath(ref targetObject, defaultPath);
			
			if (_info != null) {
				isValid = true;

				_valueGetter = () => { return _info.GetValue(targetObject, null); };
				_valueSetter = (value) => { _info.SetValue(targetObject, value, null); };
				
				if (_info.PropertyType == typeof(LayerMask)) {
					_valueGetter = () => { return ((LayerMask)_info.GetValue(targetObject, null)).value; };
					_valueSetter = (value) => {
						var lm = (LayerMask)_info.GetValue(targetObject, null);
						lm.value = (int)value;
						_info.SetValue(targetObject, lm, null);
					};
				} else
				/*if (_info.PropertyType.IsEnum) {
					_valueGetter = () => (int)_info.GetValue(targetObject, null);
				} else*/
				if (_info.PropertyType == typeof(Char)) {
					_valueGetter = () => (Char)_info.GetValue(targetObject, null);
				} else
				if (_info.PropertyType == typeof(int) ||
					_info.PropertyType == typeof(bool) ||
					_info.PropertyType == typeof(float) ||
					_info.PropertyType == typeof(string) ||
					_info.PropertyType == typeof(Color) ||
					_info.PropertyType == typeof(Vector2) ||
					_info.PropertyType == typeof(Vector3) ||
					_info.PropertyType == typeof(Vector4) ||
					_info.PropertyType == typeof(Rect) ||
					_info.PropertyType == typeof(AnimationCurve) ||
					_info.PropertyType == typeof(Bounds) ||
					_info.PropertyType == typeof(Quaternion) ||
					_info.PropertyType == typeof(Vector2Int) ||
					_info.PropertyType == typeof(Vector3Int) ||
					_info.PropertyType == typeof(RectInt) ||
					_info.PropertyType == typeof(BoundsInt) ||
					_info.PropertyType.IsEnum ||
					_info.PropertyType.IsSubclassOf(typeof(UnityEngine.Object))
					) {
				} else {
					isValid = false;
					Debug.LogError($"{propName} property type {_info.PropertyType} is not a valid type");
				}
			} else {
				//Debug.LogError($"Can't find property {defaultPath}");
			}
			_isValid = isValid;
		}
	}
}
#endif
