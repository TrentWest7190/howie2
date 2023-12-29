using System;
using UnityEngine;
using UnityEngine.UI;

namespace Nexweron.TargetRender
{
	[Serializable]
	public class RawImageTarget : SubTarget
	{
		[SerializeField]
		private Texture m_defaultTexture = null;
		
		[SerializeField] RawImage m_rawImage = null;
		private RawImage _rawImage = null;
		public RawImage rawImage {
			get => _rawImage;
			set {
				if (_rawImage == value) return;
				RevertDefaultTexture(true);
				_rawImage = m_rawImage = value;
				_isModified = true;
			}
		}
		
		public override bool isValidTarget => _rawImage != null;
		
		protected override Texture GetDefaultTexture() {
			return _rawImage.texture;
		}
		
		protected override void CacheDefaultTexture() {
			var sourceTex = GetDefaultTexture();
			
			if (_valueTexture == sourceTex) {
				// Already value, should be cached
				if (!_valueTexture) {
					// Value has been destroyed from the outside
					SetValueTexture(null);
				}
				return;
			}
			// New default
			if (_defaultTexture != sourceTex) {
				_defaultTexture = m_defaultTexture = sourceTex;
				_isCachedDefaultTexture = true;
				SetModified();
				return;
			}
			// Current = default = null
			if (!_defaultTexture) {
				_isCachedDefaultTexture = true;
			}
			// Need set value
			SetModified();
		}

		protected override void RevertDefaultTexture(bool clearCache = false) {
			if (!_isCachedDefaultTexture) return;
			// Target can be destroyed
			if (!isValidTarget) {
				_rawImage = m_rawImage = null;
				_isCachedDefaultTexture = false;
				_defaultTexture = m_defaultTexture = null;
				return;
			}
			
			SetTargetTexture(_defaultTexture);
			if (!clearCache) return;
			
			_isCachedDefaultTexture = false;
			_defaultTexture = m_defaultTexture = null;
		}
		
		public override Texture GetTargetTexture() {
			return _rawImage.texture;
		}

		protected override void SetTargetTexture(Texture texture) {
			_rawImage.texture = texture;
		}
		
		// Recover default on play 
		private void UpdateSerializedDefaultTexture() {
			if (!isValidTarget) return;
			if (_defaultTexture != m_defaultTexture) {
				_defaultTexture = m_defaultTexture;
				_isCachedDefaultTexture = true;
				SetTargetTexture(_defaultTexture);
			}
		}
		
		public override void UpdateSerialized() {
			rawImage = m_rawImage;
			UpdateSerializedDefaultTexture();
		}
	}
}