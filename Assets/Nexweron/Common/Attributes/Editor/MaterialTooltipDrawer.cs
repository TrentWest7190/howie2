using UnityEditor;
using UnityEngine;

namespace Nexweron.Common.Attributes
{
	public class MaterialTooltipDecorator : MaterialPropertyDrawer
	{
		private string _tooltip;

		public MaterialTooltipDecorator(string tooltip) {
			_tooltip = tooltip.Replace("shN", "\n");
		}

		public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor) {
			label.tooltip = _tooltip;
		}

		public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor) {
			return 0;
		}
	}
}
