using UnityEngine;
using UnityEngine.Video;

namespace Nexweron.FragFilter
{
	public class FFBridgeVideoPlayer : FFBridgeBase
	{
		public override bool isValid => videoPlayer && videoPlayer.texture && base.isValid;

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
			_videoFrame = -1;
			_isModified = true;
		}
		private long _videoFrame = -1;
		
		void LateUpdate() {
			if (!isValid) return;
			
			var texture = _videoPlayer.texture as RenderTexture;
			_ffController.SetSourceTexture(texture, this);
				
			CheckModified(_videoFrame != _videoPlayer.frame || _ffController.CheckModified());
			_videoFrame = _videoPlayer.frame;

			if (_isModified) {
				ffController.RenderOut(texture);
				_isModified = false;
			}
		}
		
		protected override void UpdateSerialized() {
			videoPlayer = m_videoPlayer;
			base.UpdateSerialized();
		}
	}
}