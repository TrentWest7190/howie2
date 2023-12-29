using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nexweron.FragFilter
{
	[CustomEditor(typeof(FFController), true)]
	public class FFControllerEditor : Editor
	{
		private string[] _excludedDrawProps = { "m_Script" };
		
		void OnEnable() {
			var targetInstance = (FFController) target;
			
			var ffComponents = targetInstance.ffComponents;
			if (!ffComponents.SequenceEqual(targetInstance.GetComponents<FFComponent>())) {
				targetInstance.UpdateComponents();
			}
		}
		
		public override void OnInspectorGUI() {
			serializedObject.Update();
			
			var targetInstance = (FFController) target;
			
			//Bridge
			var bridge = targetInstance.bridge;
			EditorGUI.BeginDisabledGroup(true);
			if (bridge != null) {
				if (bridge is Object bridgeObject) {
					EditorGUILayout.ObjectField("Bridge", bridgeObject, typeof(Object), true);
				}
				else {
					EditorGUILayout.TextField("Bridge", bridge.ToString());
				}
			}
			else {
				EditorGUILayout.ObjectField("Bridge", null, typeof(IFFBridge), false);
			}
			EditorGUI.EndDisabledGroup();
			
			//Default
			DrawPropertiesExcluding(serializedObject, _excludedDrawProps);
			serializedObject.ApplyModifiedProperties();
		}
	}
}
