using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Nexweron.TargetRender
{
	[CustomEditor(typeof(TargetValueBase), true)]
	public class TargetValueBaseEditor : Editor
	{
		SerializedProperty _scriptProperty;
		SerializedProperty _renderModeProperty;
		SerializedProperty _targetRenderTextureProperty;
		SerializedProperty _targetRendererProperty;
		SerializedProperty _targetMaterialIndexProperty;
		SerializedProperty _targetMaterialPropProperty;
		SerializedProperty _targetRawImageProperty;
		
		AnimBool _showRenderTexture = new AnimBool();
		AnimBool _showRenderer = new AnimBool();
		AnimBool _showRawImage = new AnimBool();
		
		private PopupData<int> _materialPopupData;
		private int _materialIndex = -1;
		
		private PopupData<string> _materialPropPopupData;
		private int _materialPropIndex = -1;
		private string _materialPropValue = null;
		private Renderer _renderer;
		private TargetValueBase _targetInstance;
		
		protected virtual void OnEnable() {
			_targetInstance = target as TargetValueBase;
			if (!_targetInstance) Debug.LogError($"Editor target({target.GetType()}) is not TargetValueBase");
			
			_scriptProperty = serializedObject.FindProperty("m_Script");
			_renderModeProperty = serializedObject.FindProperty("m_renderMode");
			_targetRenderTextureProperty = serializedObject.FindProperty("_renderTextureTarget.m_renderTexture");
			_targetRendererProperty = serializedObject.FindProperty("_materialOverrideTarget.m_renderer");
			_targetMaterialIndexProperty = serializedObject.FindProperty("_materialOverrideTarget.m_rendererMaterialIndex");
			_targetMaterialPropProperty = serializedObject.FindProperty("_materialOverrideTarget.m_rendererMaterialProp");
			_targetRawImageProperty = serializedObject.FindProperty("_rawImageTarget.m_rawImage");
			
			var renderMode = (TargetValueBase.RenderMode) _renderModeProperty.enumValueIndex;
			_showRenderTexture.value = (renderMode == TargetValueBase.RenderMode.RenderTexture);
			_showRenderer.value = (renderMode == TargetValueBase.RenderMode.MaterialOverride);
			_showRawImage.value = (renderMode == TargetValueBase.RenderMode.RawImage);

			_showRenderTexture.valueChanged.AddListener(Repaint);
			_showRenderer.valueChanged.AddListener(Repaint);
			_showRawImage.valueChanged.AddListener(Repaint);

			_materialPopupData = new PopupData<int>();
			_materialPopupData.labelTextContent = EditorGUIUtility.TrTextContent("Material", "Material property of the current Renderer");

			_materialPropPopupData = new PopupData<string>();
			_materialPropPopupData.labelTextContent = EditorGUIUtility.TrTextContent("Material Property", "Texture property of the current Material that will receive the images");
		}
		
		protected virtual void OnDisable() {
			_showRenderTexture.valueChanged.RemoveListener(Repaint);
			_showRenderer.valueChanged.RemoveListener(Repaint);
			_showRawImage.valueChanged.RemoveListener(Repaint);

			_materialPopupData = null;
			_materialPropPopupData = null;
			_renderer = null;
		}
		
		protected List<string> _excludedDrawProps = new List<string>();
		protected virtual List<string> GetExcludedDrawProps() {
			if (_excludedDrawProps.Any()) return _excludedDrawProps;
			_excludedDrawProps.Add(_scriptProperty.name);
			_excludedDrawProps.Add(_renderModeProperty.name);
			return _excludedDrawProps;
		}
		
		protected virtual void OnPreInspectorGUI() { }
		
		public override void OnInspectorGUI() {
			serializedObject.Update();
			
			//Default
			DrawPropertiesExcluding(serializedObject, GetExcludedDrawProps().ToArray());
			OnPreInspectorGUI();
			
			//Render
			EditorGUILayout.PropertyField(_renderModeProperty);

			var renderMode = (TargetValueBase.RenderMode) _renderModeProperty.enumValueIndex;
			_showRenderTexture.target = (!_renderModeProperty.hasMultipleDifferentValues && renderMode == TargetValueBase.RenderMode.RenderTexture);
			_showRenderer.target = (!_renderModeProperty.hasMultipleDifferentValues && renderMode == TargetValueBase.RenderMode.MaterialOverride);
			_showRawImage.target = (!_renderModeProperty.hasMultipleDifferentValues && renderMode == TargetValueBase.RenderMode.RawImage);
			
			++EditorGUI.indentLevel;
			{
				if (EditorGUILayout.BeginFadeGroup(_showRenderTexture.faded)) {
					EditorGUILayout.PropertyField(_targetRenderTextureProperty);
				}
				EditorGUILayout.EndFadeGroup();

				if (EditorGUILayout.BeginFadeGroup(_showRenderer.faded)) {
					var renderer = _targetRendererProperty.objectReferenceValue as Renderer;
					if (renderer == null) {
						renderer = _targetInstance.GetComponent<Renderer>();
						_targetRendererProperty.objectReferenceValue = renderer;
					}

					EditorGUILayout.PropertyField(_targetRendererProperty);

					bool isRendererChanged = false;
					var materialIndex = _targetMaterialIndexProperty.intValue;

					if (_renderer != renderer) {
						_renderer = renderer;

						isRendererChanged = true;
						UpdateMaterialPopupData(renderer);

						if (_materialPopupData.isEmpty) {
							materialIndex = -1;
						}
						else if (materialIndex < 0 || materialIndex >= _materialPopupData.itemsCount) {
							materialIndex = 0;
						}

						SetMaterialIndex(materialIndex);
					}

					void SetMaterialIndex(int value) {
						_targetInstance.targetRendererMaterialIndex = value; // set value by instance setter
						serializedObject.Update(); // (instance => editor)

						// hack for editor change events (OnValidate, etc)
						var updatedValue = _targetMaterialIndexProperty.intValue;
						_targetMaterialIndexProperty.intValue = int.MinValue;
						_targetMaterialIndexProperty.intValue = updatedValue;
					}

					if (_materialPopupData.itemsCount > 1) {
						EditorGUI.indentLevel++;
						materialIndex = HandlePopup(_targetMaterialIndexProperty, _materialPopupData, SetMaterialIndex, materialIndex);
						EditorGUI.indentLevel--;
					}

					if (isRendererChanged || _materialIndex != materialIndex) {
						_materialPropValue = _targetMaterialPropProperty.stringValue;
						_materialIndex = materialIndex;

						Material targetMaterial = null;
						if (_renderer != null && _materialIndex >= 0 && _materialIndex < renderer.sharedMaterials.Length) {
							targetMaterial = renderer.sharedMaterials[_materialIndex];
						}
						
						UpdateMaterialPropPopupData(targetMaterial);
						_materialPropIndex = _materialPropPopupData.GetItemIndex(_targetMaterialPropProperty.stringValue);

						if (_materialPropPopupData.isEmpty) {
							_materialPropIndex = -1;
							SetMaterialProp(null);
						}
						else if (_materialPropIndex < 0) {
							_materialPropIndex = 0;
							SetMaterialProp(_materialPropPopupData.GetItemValue(0));
						}
					}

					void SetMaterialProp(string value) {
						_targetInstance.targetRendererMaterialProp = value; // set value by instance setter
						serializedObject.Update(); // (instance => editor)
						// hack for editor change events (OnValidate, etc)
						var updatedValue = _targetMaterialPropProperty.stringValue;
						_targetMaterialPropProperty.stringValue = String.Empty;
						_targetMaterialPropProperty.stringValue = updatedValue;
					}

					var materialPropIndex = _materialPropIndex;
					if (_materialPropValue != _targetMaterialPropProperty.stringValue) {
						materialPropIndex = _materialPropPopupData.GetItemIndex(_targetMaterialPropProperty.stringValue);
					}
					
					_materialPropIndex = HandlePopup(_targetMaterialPropProperty, _materialPropPopupData, SetMaterialProp, materialPropIndex);
				}
				EditorGUILayout.EndFadeGroup();
				
				if (EditorGUILayout.BeginFadeGroup(_showRawImage.faded)) {
					var rawImage = _targetInstance.GetComponent<RawImage>();
					if (rawImage) {
						EditorGUI.BeginDisabledGroup(true);
						EditorGUILayout.ObjectField("RawImage", rawImage, typeof(RawImage), false);
						EditorGUI.EndDisabledGroup();
					}
					else {
						EditorGUILayout.HelpBox("RawImage component not found", MessageType.Warning);
					}
				}
				EditorGUILayout.EndFadeGroup();
			}
			--EditorGUI.indentLevel;
			
			serializedObject.ApplyModifiedProperties();
		}
		
		// Helpers
		private class PopupData<T>
		{
			private List<string> _names = new List<string>();
			private List<T> _values = new List<T>();

			private GUIContent[] _namesContent;
			public GUIContent[] namesContent => _namesContent ?? (_namesContent = _names.Select(x => new GUIContent(x)).ToArray());

			public bool isEmpty => _values.Count == 0;
			public int itemsCount => _values.Count;
			public GUIContent labelTextContent;

			public void AddItem(string displayName, T value) {
				_namesContent = null;
				_names.Add(displayName);
				_values.Add(value);
			}

			public bool ContainsItem(T value) {
				return _values.Contains(value);
			}

			public T GetItemValue(int index) {
				return _values[index];
			}

			public int GetItemIndex(T value) {
				return _values.IndexOf(value);
			}

			public void ClearItems() {
				_namesContent = null;
				_names.Clear();
				_values.Clear();
			}
		}

		private void UpdateMaterialPopupData(Renderer renderer) {
			var popupData = _materialPopupData;
			popupData.ClearItems();

			if (!renderer) return;

			for (int i = 0; i < renderer.sharedMaterials.Length; i++) {
				var mat = renderer.sharedMaterials[i];
				var name = mat ? mat.name : "None";
				popupData.AddItem(name, i);
			}
		}

		private void UpdateMaterialPropPopupData(Material material) {
			var popupData = _materialPropPopupData;
			popupData.ClearItems();

			if (!material) return;

			var propertyCount = material.shader.GetPropertyCount();
			for (var i = 0; i < propertyCount; i++) {
				if (material.shader.GetPropertyType(i) == ShaderPropertyType.Texture) {
					var propertyName = material.shader.GetPropertyName(i);
					if (!popupData.ContainsItem(propertyName))
						popupData.AddItem(propertyName, propertyName);
				}
			}
		}

		private int HandlePopup<T>(SerializedProperty property, PopupData<T> popupData, Action<T> propertySetter, int selectedIndex) where T : IConvertible {
			GUILayout.BeginHorizontal();
			var pos = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
			var label = EditorGUI.BeginProperty(pos, popupData.labelTextContent, property);

			EditorGUI.BeginChangeCheck();
			EditorGUI.BeginDisabledGroup(popupData.isEmpty);
			selectedIndex = EditorGUI.Popup(pos, label, selectedIndex, popupData.namesContent);
			EditorGUI.EndDisabledGroup();

			if (EditorGUI.EndChangeCheck()) {
				propertySetter.Invoke(popupData.GetItemValue(selectedIndex));
			}

			EditorGUI.EndProperty();
			GUILayout.EndHorizontal();
			return selectedIndex;
		}
	}
}
