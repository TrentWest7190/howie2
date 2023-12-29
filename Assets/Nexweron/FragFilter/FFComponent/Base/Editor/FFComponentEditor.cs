using UnityEditor;

namespace Nexweron.FragFilter
{
	[CustomEditor(typeof(FFComponent), true)]
	public class FFComponentEditor : Editor
	{
		protected string[] _excludedDrawProps = {"m_Script"};

		public override void OnInspectorGUI() {
			serializedObject.Update();
			DrawPropertiesExcluding(serializedObject, _excludedDrawProps);
			serializedObject.ApplyModifiedProperties();
		}
	}
}
