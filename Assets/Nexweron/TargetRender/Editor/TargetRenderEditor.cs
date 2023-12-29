using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Nexweron.TargetRender
{
	[CustomEditor(typeof(TargetRender), true)]
	public class TargetRenderEditor : TargetValueBaseEditor
	{
		SerializedProperty _sourceModeProperty;
		SerializedProperty _sourceTextureProperty;
		
		protected override void OnEnable() {
			base.OnEnable();

			_sourceModeProperty = serializedObject.FindProperty("m_sourceMode");
			_sourceTextureProperty = serializedObject.FindProperty("m_sourceTexture");
		}

		protected override List<string> GetExcludedDrawProps() {
			if (_excludedDrawProps.Any()) return _excludedDrawProps;
			base.GetExcludedDrawProps();
			_excludedDrawProps.Add(_sourceModeProperty.name);
			_excludedDrawProps.Add(_sourceTextureProperty.name);
			return _excludedDrawProps;
		}
		
		protected override void OnPreInspectorGUI() {
			var targetInstance = target as TargetRender;
			if (!targetInstance) {
				Debug.LogError($"Editor target({target.GetType()}) is not TargetRender");
				return;
			}
			
			var sourceMode = (TargetRender.SourceMode) _sourceModeProperty.enumValueIndex;
			var isShowSourceTexture = !_sourceModeProperty.hasMultipleDifferentValues &&
			                            sourceMode == TargetRender.SourceMode.Manual;

			EditorGUI.BeginDisabledGroup(!targetInstance.availableSourceFromTarget);
				EditorGUILayout.PropertyField(_sourceModeProperty);
			EditorGUI.EndDisabledGroup();
			
			if (isShowSourceTexture) {
				++EditorGUI.indentLevel;
				EditorGUILayout.PropertyField(_sourceTextureProperty);
				--EditorGUI.indentLevel;
			}
			base.OnPreInspectorGUI();
		}
	}
}

