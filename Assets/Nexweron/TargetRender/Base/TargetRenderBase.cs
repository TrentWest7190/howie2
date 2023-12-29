using UnityEngine;

namespace Nexweron.TargetRender
{
	[DisallowMultipleComponent]
	public class TargetRenderBase : TargetValueBase
	{
		protected bool _isModified = false;
		protected bool CheckModified(bool checker) {
			return _isModified = _isModified || checker;
		}
		
		// Render
		protected virtual Texture GetValueRender() {
			return null;
		}
		
		private void PreRenderValueTexture(bool isForce = false) {
			CheckModified(isForce);
			SetValueTexture(GetValueRender());
			_isModified = false;
		}
		
		public void Render(bool isForce = false) {
			if (!ReCacheDefault()) return;
			PreRenderValueTexture(isForce);
			RenderValue();
		}
		
		// Update
		protected virtual void LateUpdate() {
			Render();
		}
	}
}
