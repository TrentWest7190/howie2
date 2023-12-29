using Nexweron.Common.Attributes;
using Nexweron.TargetRender;
using UnityEngine;
using UnityEngine.Video;

namespace Nexweron.FragFilter
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Nexweron/FragFilter/FF VideoPlayerRender")]
	public class FFVideoPlayerRender : TargetRenderBase, IFFBridge
	{
		protected override bool isProvided => (ffController || videoPlayer) && base.isProvided;
		
		// VideoPlayer
		[SerializeField]
		private VideoPlayer m_videoPlayer;
		protected VideoPlayer _videoPlayer;
		public VideoPlayer videoPlayer {
			get => _videoPlayer;
			set {
				if (_videoPlayer != value) {
					_videoPlayer = m_videoPlayer = value;
					UpdateVideoPlayer();
				}
			}
		}
		private void UpdateVideoPlayer() {
			UpdateProvided();
			_videoFrame = -1;
			_isModified = true;
		}
		private long _videoFrame = -1;
		
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
			
			if (_videoPlayer) {
				if (_videoPlayer.texture) {
					CheckModified(_videoFrame != _videoPlayer.frame);
					_videoFrame = _videoPlayer.frame;
					texture = _videoPlayer.texture;
				}
			} else _videoPlayer = null;
			
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
			videoPlayer = m_videoPlayer;
			ffController = m_ffController;
		}
	}
}