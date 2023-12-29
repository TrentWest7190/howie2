using System;
using UnityEngine;

namespace Nexweron.TargetRender
{
	[Serializable]
	public class RenderTextureTarget : SubTarget
	{
		[SerializeField] RenderTexture m_renderTexture;
		private RenderTexture _renderTexture;
		public RenderTexture renderTexture {
			get => _renderTexture;
			set {
				if (_renderTexture == value) return;
				RevertDefaultTexture();
				_renderTexture = m_renderTexture = value;
				_isModified = true;
			}
		}
		
		protected uint _valueTextureUpdateCount = uint.MinValue;
		public override bool isValidTarget => _renderTexture != null;
		
		protected override void CacheDefaultTexture() {
			_isCachedDefaultTexture = true;
		}
		
		protected override void RevertDefaultTexture(bool clearCache = false) {
			_isCachedDefaultTexture = false;
		}
		
		public override Texture GetTargetTexture() {
			return _valueTexture;
		}
		
		public override void SetValueTexture(Texture texture) {
			base.SetValueTexture(texture);
			if(_isModified || !texture) return;
			
			var updateCount = texture.updateCount;
			if (_valueTextureUpdateCount == updateCount) return;
			
			_valueTextureUpdateCount = updateCount;
			_isModified = true;
		}
		
		protected override void SetTargetTexture(Texture texture) {
			_renderTexture.DiscardContents();
			Graphics.Blit(_valueTexture, _renderTexture);
			_renderTexture.IncrementUpdateCount();
		}
		
		public override void UpdateSerialized() {
			renderTexture = m_renderTexture;
		}
	}
}