using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Nexweron.Common.Attributes
{
	public class MaterialIfKeywordDrawer : MaterialPropertyDrawer
	{
		private string[] _keywords;
		protected bool _value = true;
		
		public MaterialIfKeywordDrawer(string k1, string k2, string k3) {
			Base(new string[3] {k1, k2, k3});
		}

		public MaterialIfKeywordDrawer(string k1, string k2) {
			Base(new string[2] {k1, k2});
		}

		public MaterialIfKeywordDrawer(string keyword) {
			Base(new string[1] {keyword});
		}

		protected virtual void Base(string[] keywords, bool value = true) {
			_keywords = keywords;
			_value = value;
		}

		protected bool IsAnyKeywordsEquals(MaterialEditor editor) {
			bool isEquals = false;
			foreach (var target in editor.targets) {
				var material = (Material) target;
				if (material) {
					var isAnyKeywordEnabled = _keywords.Any(k => material.IsKeywordEnabled(k));
					isEquals |= (_value == isAnyKeywordEnabled);
				}
			}
			return isEquals;
		}
	}
}
