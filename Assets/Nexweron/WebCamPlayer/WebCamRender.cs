using UnityEngine;
using Nexweron.TargetRender;

namespace Nexweron.WebCamPlayer
{
	[DisallowMultipleComponent]
	public class WebCamRender : TargetRenderBase
	{
		protected override bool isProvided => webCamStream && base.isProvided;
		
		[SerializeField] WebCamStream m_webCamStream;
		protected WebCamStream _webCamStream;
		public WebCamStream webCamStream {
			get => _webCamStream;
			set {
				if (_webCamStream == value) return;
				_webCamStream = m_webCamStream = value;
				UpdateWebCamStream();
			}
		}
		private void UpdateWebCamStream() {
			UpdateProvided();
			_isModified = true;
		}
		
		// Render
		protected override Texture GetValueRender() {
			var texture = base.GetValueRender();
			if (_webCamStream) {
				if (_webCamStream.webCamTexture) {
					CheckModified(_webCamStream.didUpdateThisFrame);
					texture = _webCamStream.webCamTexture;
				}
			} else _webCamStream = null;
			return texture;
		}
		
		// Update
		protected override void UpdateSerialized() {
			base.UpdateSerialized();
			webCamStream = m_webCamStream;
		}
	}
}