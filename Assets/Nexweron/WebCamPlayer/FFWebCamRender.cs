using Nexweron.Common.Attributes;
using UnityEngine;
using Nexweron.WebCamPlayer;

namespace Nexweron.FragFilter
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Nexweron/FragFilter/FF WebCamRender")]
	public class FFWebCamRender : WebCamRender, IFFBridge
	{
		protected override bool isProvided => (ffController && isActiveAndEnabled) || base.isProvided;
		
		// FFController
		[SerializeField][InspectorRename("FF Controller")]
		private FFController m_ffController;
		protected FFController _ffController;
		
		public FFController ffController {
			get => _ffController;
			set {
				if (_ffController != value) {
					_ffController = value ? value.TrySetBridge(this) : _ffController.SetBridgeNull(this);
					m_ffController = _ffController;
					UpdateFFController();
				}
				// bridge can be lost on deserialization
				else if (_ffController) _ffController.TrySetBridge(this);
			}
		}
		private void UpdateFFController() {
			UpdateProvided();
			_isModified = true;
		}
		
		// Render
		protected override Texture GetValueRender() {
			var texture = base.GetValueRender();
			
			if (!_ffController) { _ffController = null; return texture; }
			if (!_ffController.isActiveAndEnabled) return texture;
			
			texture = texture ? texture : defaultTexture;
			_ffController.SetSourceTexture(texture, this);
			if (_isModified || _ffController.CheckModified()) {
				_isModified = true;
				return _ffController.RenderIn();
			}
			
			return valueTexture;
		}
		
		// Update
		protected override void UpdateSerialized() {
			base.UpdateSerialized();
			ffController = m_ffController;
		}
	}
}