using System;
using System.Linq;
using UnityEngine;

namespace Nexweron.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public abstract class InspectorIfAttribute : InspectorAttribute
	{
		protected readonly object[] _values = { true };
		private readonly bool _inverse = false;
		
		protected InspectorIfAttribute(bool inverse) : base() {
			_inverse = inverse;
		}
		
		protected InspectorIfAttribute(object value) : base() {
			_values = new[] {value};
		}
		protected InspectorIfAttribute(object value, bool inverse) : base() {
			_values = new[] {value};
			_inverse = inverse;
		}
		protected InspectorIfAttribute(object v1, object v2) : base() {
			_values = new[] {v1, v2};
		}
		protected InspectorIfAttribute(object v1, object v2, bool inverse) : base() {
			_values = new[] {v1, v2};
			_inverse = inverse;
		}
		protected InspectorIfAttribute(object v1, object v2, object v3) : base() {
			_values = new[] {v1, v2, v3};
		}
		protected InspectorIfAttribute(object v1, object v2, object v3, bool inverse) : base() {
			_values = new[] {v1, v2, v3};
			_inverse = inverse;
		}

	#if UNITY_EDITOR
		protected bool _isValid = false;
		protected virtual bool isEquals => _values.All(v => v is bool);
		protected bool isOn => _inverse ? !isEquals : isEquals;
		
		public override void Init(UnityEditor.SerializedProperty field, System.Reflection.FieldInfo fieldInfo) {
			_isValid = _values[0] is bool;
			if (!_isValid) {
				Debug.LogError($"Value '{_values[0].GetType()}) type mismatch ({typeof(bool)})");
			}
		}
	#endif
	}

	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public abstract class InspectorTargetIfAttribute : InspectorIfAttribute
	{
		private string _targetName = null;
		
		public InspectorTargetIfAttribute(string targetName, object value) : base(value) {
			_targetName = targetName;
		}
		protected InspectorTargetIfAttribute(string targetName, object value, bool inverse) : base(value, inverse) {
			_targetName = targetName;
		}
		public InspectorTargetIfAttribute(string targetName, object v1, object v2) : base(v1, v2) {
			_targetName = targetName;
		}
		protected InspectorTargetIfAttribute(string targetName, object v1, object v2, bool inverse) : base(v1, v2, inverse) {
			_targetName = targetName;
		}
		public InspectorTargetIfAttribute(string targetName, object v1, object v2, object v3) : base(v1, v2, v3) {
			_targetName = targetName;
		}
		protected InspectorTargetIfAttribute(string targetName, object v1, object v2, object v3, bool inverse) : base(v1, v2, v3, inverse) {
			_targetName = targetName;
		}

	#if UNITY_EDITOR
		private Utils.FieldModifier _targetFieldModifier;
		private Utils.PropertyModifier _targetPropModifier;
		private Func<object> _targetValueGetter = () => null;
		
		protected override bool isEquals => _values.Contains(_targetValueGetter.Invoke());

		public override void Init(UnityEditor.SerializedProperty field, System.Reflection.FieldInfo fieldInfo) {
			_isValid = false;
			var propAttr = this;
			
			_targetPropModifier = new Utils.PropertyModifier(field.serializedObject.targetObject, propAttr._targetName);
			if (_targetPropModifier.isValid) {
				foreach (var v in _values) {
					if (_targetPropModifier.info.PropertyType == v.GetType()) continue;
					Debug.LogError($"Property '{_targetName}'({_targetPropModifier.info.PropertyType}) mismatch value type ({v.GetType()})");
					return;
				}
				_isValid = true;
				_targetValueGetter = () => _targetPropModifier.GetValue();
				return;
			}
			
			_targetFieldModifier = new Utils.FieldModifier(field.serializedObject.targetObject, propAttr._targetName);
			if (_targetFieldModifier.isValid) {
				foreach (var v in _values) {
					if (_targetFieldModifier.info.FieldType == v.GetType()) continue;
					Debug.LogError($"Field '{_targetName}' ({_targetFieldModifier.info.FieldType}) mismatch value type ({v.GetType()})");
					return;
				}
				_isValid = true;
				_targetValueGetter = () => _targetFieldModifier.GetValue();
				return;
			}
			Debug.LogError("Can't find field or property " + _targetName);
		}
	#endif
	}
}