using UnityEngine;
using UnityEditor;

namespace Nexweron.Common.Attributes
{
	public class MaterialDisableIfKeywordDrawer : MaterialIfKeywordDrawer
	{
		public MaterialDisableIfKeywordDrawer(string keyword) : base(keyword) { }
		public MaterialDisableIfKeywordDrawer(string k1, string k2) : base(k1, k2) { }
		public MaterialDisableIfKeywordDrawer(string k1, string k2, string k3) : base(k1, k2, k3) { }

		public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor) {
			var isDisable = IsAnyKeywordsEquals(editor);
			
			if (isDisable) { GUI.enabled = false; }
			EditorGUI.PrefixLabel(position, label);
			editor.DefaultShaderProperty(prop, null);
			if (isDisable) { GUI.enabled = true; }
		}

		public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor) {
			return -EditorGUIUtility.standardVerticalSpacing;
		}
	}
	
	public class MaterialEnableIfKeywordDrawer : MaterialDisableIfKeywordDrawer
	{
		public MaterialEnableIfKeywordDrawer(string keyword) : base(keyword) { }
		public MaterialEnableIfKeywordDrawer(string k1, string k2) : base(k1, k2) { }
		public MaterialEnableIfKeywordDrawer(string k1, string k2, string k3) : base(k1, k2, k3) { }

		protected override void Base(string[] keywords, bool value = true) {
			base.Base(keywords, value);
			_value = !_value;
		}
	}
}
