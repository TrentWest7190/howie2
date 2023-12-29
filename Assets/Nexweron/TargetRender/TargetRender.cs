using UnityEngine;

namespace Nexweron.TargetRender
{
	[DisallowMultipleComponent]
	public class TargetRender : TargetRenderBase
	{
		// Source mode
		public enum SourceMode {
			FromTarget,
			Manual
		}
		
		[SerializeField]
		private SourceMode m_sourceMode = SourceMode.Manual;
		private SourceMode _sourceMode = SourceMode.Manual;
		public SourceMode sourceMode {
			get => _sourceMode;
			set {
				if (_sourceMode == value) return;
				if (value == SourceMode.Manual || availableSourceFromTarget) {
					_sourceMode = m_sourceMode = value;
					_isModified = true;
				}
				else {
					m_sourceMode = _sourceMode;
					Debug.LogWarning($"{value} is not available in {renderMode} mode");
				}
			}
		}
		
		// Source texture
		[SerializeField]
		private Texture m_sourceTexture;
		private Texture _sourceTexture;
		public Texture sourceTexture {
			get => _sourceTexture;
			set {
				if (_sourceTexture == value) return;
				_sourceTexture = m_sourceTexture = value;
				if (sourceMode == SourceMode.Manual) _isModified = true;
			}
		}

		public bool availableSourceFromTarget => _renderMode == RenderMode.MaterialOverride ||
		                                         _renderMode == RenderMode.RawImage;
		
		// Render mode
		protected override void UpdateRenderMode() {
			if (!availableSourceFromTarget) {
				sourceMode = SourceMode.Manual;
			}
			base.UpdateRenderMode();
		}

		// Render
		protected override Texture GetValueRender() {
			if (sourceMode == SourceMode.Manual) return _sourceTexture;
			return defaultTexture;
		}
		
		protected override void UpdateSerialized() {
			base.UpdateSerialized();
			sourceMode = m_sourceMode;
			sourceTexture = m_sourceTexture;
		}
	}
}