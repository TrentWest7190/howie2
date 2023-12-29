using UnityEngine;
using UnityEditor;

namespace Nexweron.Common.Attributes
{
	public class MaterialHideIfKeywordDrawer : MaterialIfKeywordDrawer
	{
		public MaterialHideIfKeywordDrawer(string keyword) : base(keyword) { }
		public MaterialHideIfKeywordDrawer(string k1, string k2) : base(k1, k2) { }
		public MaterialHideIfKeywordDrawer(string k1, string k2, string k3) : base(k1, k2, k3) { }
		
		public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor) {
			var isHidden = IsAnyKeywordsEquals(editor);
			if (!isHidden) {
				EditorGUI.PrefixLabel(position, label);
				editor.DefaultShaderProperty(prop, null);
			}
		}
		
		public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor) {
			return -EditorGUIUtility.standardVerticalSpacing;
		}
	}

	public class MaterialShowIfKeywordDrawer : MaterialHideIfKeywordDrawer
	{
		public MaterialShowIfKeywordDrawer(string keyword) : base(keyword) { }
		public MaterialShowIfKeywordDrawer(string k1, string k2) : base(k1, k2) { }
		public MaterialShowIfKeywordDrawer(string k1, string k2, string k3) : base(k1, k2, k3) { }

		protected override void Base(string[] keywords, bool value = true) {
			base.Base(keywords, value);
			_value = !_value;
		}
	}
}